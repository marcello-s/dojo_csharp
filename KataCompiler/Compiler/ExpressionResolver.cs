#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Compiler;

class ExpressionResolver : BaseExpressionVisitor
{
    public ICompilerErrorReporter ErrorReporter { get; private set; }

    public ExpressionResolver()
    {
        ErrorReporter = new CompilerErrorReporter();
    }

    public override IExpression Visit(AccessorExpression expr, Scope scope)
    {
        var ae = (AccessorExpression)base.Visit(expr, scope);

        if (ae.Member is IdentifierExpression || ae.Member is IdentifierPartExpression)
        {
            var memberIdentifier =
                ae.Member is IdentifierPartExpression
                    ? RenderIdentifierParts((IdentifierPartExpression)ae.Member)
                    : ((IdentifierExpression)ae.Member).Name;
            if (scope.Lookup(memberIdentifier) == null)
            {
                ErrorReporter.AddWarning(ae.Member, "identifier not defined");
            }
        }

        return ae;
    }

    public override IExpression Visit(AssignExpression expr, Scope scope)
    {
        var ae = (AssignExpression)base.Visit(expr, scope);

        // define when in allocation mode
        var identifier = ae.Left as IdentifierExpression;
        if (scope.IsAllocationMode)
        {
            if (identifier != null && !scope.Define(identifier.Name, identifier))
            {
                ErrorReporter.AddError(identifier, "duplicate definition");
            }
        }

        if (identifier != null)
        {
            var rootName = GetRootIdentifierName(identifier.Name);
            if (scope.Lookup(rootName) != null)
            {
                ae.Tag = scope.Assign(ae.Left, ae.Right);
            }
            else
            {
                ErrorReporter.AddError(identifier, "identifier not defined");
            }
        }

        var accessor = ae.Left as AccessorExpression;
        if (
            accessor != null
            && (
                accessor.Member is IdentifierExpression
                || accessor.Member is IdentifierPartExpression
            )
        )
        {
            var identifierName =
                accessor.Member is IdentifierPartExpression
                    ? RenderIdentifierParts((IdentifierPartExpression)accessor.Member)
                    : ((IdentifierExpression)accessor.Member).Name;
            var rootName = GetRootIdentifierName(identifierName);
            if (scope.Lookup(rootName) != null)
            {
                ae.Tag = scope.Assign(ae.Left, ae.Right);
            }
            else
            {
                ErrorReporter.AddError(accessor.Member, "identifier not defined");
            }
        }

        return ae;
    }

    public override IExpression Visit(BinaryOperatorExpression expr, Scope scope)
    {
        var boe = (BinaryOperatorExpression)base.Visit(expr, scope);

        if (boe.Left is ConstantExpression && boe.Right is ConstantExpression)
        {
            var leftConst = (ConstantExpression)boe.Left;
            var rightConst = (ConstantExpression)boe.Right;

            if (leftConst.Type == rightConst.Type)
            {
                IExpression? constExpr = null;

                switch (leftConst.Type)
                {
                    case ConstantType.Number:
                        if (Op.NumberMap.ContainsKey(expr.TokenId))
                        {
                            var func = Op.NumberMap[expr.TokenId];
                            var number = func(leftConst.ToNumber(), rightConst.ToNumber());
                            constExpr = new ConstantExpression(number);
                        }
                        else
                        {
                            var func = Op.NumberComparisonMap[expr.TokenId];
                            var result = func(leftConst.ToNumber(), rightConst.ToNumber());
                            constExpr = new ConstantExpression(result);
                        }
                        break;

                    case ConstantType.String:
                        var text = Op.Concat(leftConst.Constant!, rightConst.Constant!);
                        constExpr = new ConstantExpression(text, ConstantType.String);
                        break;

                    case ConstantType.Boolean:
                        {
                            var func = Op.BooleanComparisonMap[expr.TokenId];
                            var result = func(leftConst.ToBoolean(), rightConst.ToBoolean());
                            constExpr = new ConstantExpression(result);
                        }
                        break;
                }

                return constExpr!;
            }

            // type mismatch
            ErrorReporter.AddError(boe.Left, "type mismatch");
            ErrorReporter.AddError(boe.Right, "type mismatch");
        }

        if (expr.TokenId == Parser.Token.In && scope.IsAllocationMode)
        {
            var leftIdentifier = (IdentifierExpression)expr.Left;
            scope.Define(leftIdentifier.Name, leftIdentifier);

            var rightIdentifier = (IdentifierExpression)expr.Right;
            if (scope.Lookup(rightIdentifier.Name) == null)
            {
                ErrorReporter.AddError(rightIdentifier, "identifier not defined");
            }
        }

        return boe;
    }

    public override IExpression Visit(CallExpression expr, Scope scope)
    {
        var ce = (CallExpression)base.Visit(expr, scope);

        if (ce.Function is IdentifierExpression || ce.Function is IdentifierPartExpression)
        {
            var functionIdentifier =
                ce.Function is IdentifierPartExpression
                    ? RenderIdentifierParts((IdentifierPartExpression)ce.Function)
                    : ((IdentifierExpression)ce.Function).Name;
            if (scope.Lookup(functionIdentifier) == null)
            {
                ErrorReporter.AddWarning(expr, "undefined function");
            }
        }

        return ce;
    }

    public override IExpression Visit(ConditionalExpression expr, Scope scope)
    {
        var condition = Evaluate(expr.Condition, scope);

        if (
            condition is ConstantExpression
            && ((ConstantExpression)condition).Type == ConstantType.Boolean
        )
        {
            // only evaluate the statically reachable branch
            var staticBranch = ((ConstantExpression)condition).ToBoolean()
                ? Evaluate(expr.TruthyBranch, scope)
                : Evaluate(expr.FalsyBranch, scope);
            return staticBranch;
        }

        var truthyBranch = Evaluate(expr.TruthyBranch, scope);
        var falsyBranch = Evaluate(expr.FalsyBranch, scope);

        return new ConditionalExpression(condition, truthyBranch, falsyBranch);
    }

    public override IExpression Visit(DefinitionExpression expr, Scope scope)
    {
        var de = (DefinitionExpression)base.Visit(expr, scope);
        var identifierName =
            de.IdentifierExpr is ConstantExpression
                ? ((ConstantExpression)expr.IdentifierExpr).Constant
                : ((IdentifierExpression)expr.IdentifierExpr).Name;
        scope.Define(identifierName ?? "undefined", expr.IdentifierExpr);

        var tag = new List<object?>();
        var objectLiteralExpr = de.DefinitionExpr as ObjectLiteralExpression;
        if (objectLiteralExpr != null)
        {
            var sequenceExpr = (SequenceExpression)objectLiteralExpr.Definitions;
            foreach (var exp in sequenceExpr.Exprs)
            {
                var definitionExpr = exp as DefinitionExpression;
                if (definitionExpr != null)
                {
                    tag.Add(definitionExpr.Tag);
                }
            }
        }

        var constantExpr = de.DefinitionExpr as ConstantExpression;
        if (constantExpr != null)
        {
            tag.Add(constantExpr.Constant);
        }

        de.Tag = new Scope.Pair(identifierName ?? "NULL-identifier", tag.ToArray());

        return de;
    }

    public override IExpression Visit(IdentifierPartExpression expr, Scope scope)
    {
        if (expr.Left is IdentifierPartExpression)
        {
            var name = RenderIdentifierParts(expr);

            var rootName = GetRootIdentifierName(name);
            if (scope.Lookup(rootName) == null)
            {
                ErrorReporter.AddError(expr, "identifier not defined");
            }

            return new IdentifierExpression(name);
        }

        return expr;
    }

    private static string RenderIdentifierParts(IdentifierPartExpression expr)
    {
        var currentPart = expr;
        IdentifierPartExpression? previousPart = null;
        IdentifierExpression? identifier;
        var identifiers = new List<string>();

        while (currentPart != null)
        {
            identifier = currentPart.Exprs as IdentifierExpression;
            if (identifier != null)
            {
                identifiers.Add(identifier.Name);
            }

            previousPart = currentPart;
            currentPart = currentPart.Left as IdentifierPartExpression;
        }

        if (previousPart != null)
        {
            identifier = previousPart.Left as IdentifierExpression;
            if (identifier != null)
            {
                identifiers.Add(identifier.Name);
            }
        }

        identifiers.Reverse();
        return string.Join(".", identifiers);
    }

    private static string GetRootIdentifierName(string name)
    {
        var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        return parts[0];
    }

    public override IExpression Visit(IfExpression expr, Scope scope)
    {
        var conditionalExpr = Evaluate(expr.ConditionalExpr, scope);

        if (
            conditionalExpr is ConstantExpression
            && ((ConstantExpression)conditionalExpr).Type == ConstantType.Boolean
        )
        {
            // only evaluate the statically reachable branch
            var staticBranch = ((ConstantExpression)conditionalExpr).ToBoolean()
                ? Evaluate(expr.Expr, scope)
                : Evaluate(expr.ElseExpr, scope);
            return staticBranch;
        }

        var exp = Evaluate(expr.Expr, scope);
        var elseExpr = Evaluate(expr.ElseExpr, scope);

        return new IfExpression(conditionalExpr, exp, elseExpr);
    }

    public override IExpression Visit(IllegalExpression expr, Scope scope)
    {
        ErrorReporter.AddError(expr, "illegal expression detected");

        return expr;
    }

    public override IExpression Visit(MethodExpression expr, Scope scope)
    {
        if (!scope.Define(expr.Name, expr))
        {
            ErrorReporter.AddError(expr, "duplicate definition");
        }

        scope.Push();

        var args = (SequenceExpression)Evaluate(expr.Args, scope);
        foreach (IdentifierExpression arg in args.Exprs)
        {
            if (!scope.Define(arg.Name, arg))
            {
                ErrorReporter.AddError(arg, "duplicate definition");
            }
        }

        var body = Evaluate(expr.Body, scope);

        scope.Pop();

        return new MethodExpression(expr.Name, args, body);
    }

    public override IExpression Visit(ObjectLiteralExpression expr, Scope scope)
    {
        var ole = (ObjectLiteralExpression)base.Visit(expr, scope);

        if (scope.IsAllocationMode)
        {
            var sequence = (SequenceExpression)ole.Definitions;
            foreach (DefinitionExpression definitionExpr in sequence.Exprs)
            {
                var tag = definitionExpr.Tag as Scope.Pair;
                if (tag == null)
                {
                    continue;
                }

                var objects = RenderObjectNames(tag);
                foreach (var obj in objects)
                {
                    scope.Define(obj, null!);
                }
            }
        }

        return ole;
    }

    private static IEnumerable<string> RenderObjectNames(
        Scope.Pair tag,
        IList<string>? objects = null,
        string partialName = ""
    )
    {
        if (objects == null)
        {
            objects = new List<string>();
        }

        var name = string.IsNullOrEmpty(partialName) ? partialName : partialName + ".";

        var objs = tag.Obj as object[];
        if (objs != null)
        {
            foreach (var obj in objs)
            {
                var pair = obj as Scope.Pair;
                if (pair != null)
                {
                    RenderObjectNames(pair, objects, name + tag.Key);
                }
                else
                {
                    var fullName = name + tag.Key;
                    if (!objects.Contains(fullName))
                    {
                        objects.Add(fullName);
                    }
                }
            }
        }

        return objects;
    }

    public override IExpression Visit(PrefixExpression expr, Scope scope)
    {
        var pe = (PrefixExpression)base.Visit(expr, scope);

        if (pe.Right is ConstantExpression)
        {
            var rightConst = (ConstantExpression)pe.Right;
            IExpression? constExpr = null;

            switch (rightConst.Type)
            {
                case ConstantType.Number:
                    if (Op.UnaryNumberMap.ContainsKey(pe.TokenId))
                    {
                        var func = Op.UnaryNumberMap[pe.TokenId];
                        var number = func(rightConst.ToNumber());
                        constExpr = new ConstantExpression(number);
                    }
                    break;

                case ConstantType.Boolean:
                    if (pe.TokenId == Parser.Token.Exclamation)
                    {
                        var result = Op.BooleanNot(rightConst.ToBoolean());
                        constExpr = new ConstantExpression(result);
                    }
                    break;
            }

            return constExpr!;
        }

        return pe;
    }

    public override IExpression Visit(SequenceExpression expr, Scope scope)
    {
        var unreachableCodeDetected = false;
        var exprs = new List<IExpression>();
        foreach (var exp in expr.Exprs)
        {
            if (unreachableCodeDetected)
            {
                ErrorReporter.AddWarning(exp, "unreachable code");
            }
            else
            {
                exprs.Add(Evaluate(exp, scope));
            }

            // remove statically unreachable expressions after break, continue and return
            if (
                exp is BreakExpression
                || exp is ContinueExpression
                || exp is ReturnExpression
                || exp is ThrowExpression
            )
            {
                unreachableCodeDetected = true;
            }
        }

        return new SequenceExpression(exprs);
    }

    public override IExpression Visit(TryCatchFinallyExpression expr, Scope scope)
    {
        var tcfe = (TryCatchFinallyExpression)base.Visit(expr, scope);
        var catchIdentifier = (IdentifierExpression)tcfe.CatchVariable;

        scope.Define(catchIdentifier.Name, tcfe.CatchVariable);

        return tcfe;
    }

    public override IExpression Visit(VarExpression expr, Scope scope)
    {
        scope.SetAllocationMode();
        var exprs = (SequenceExpression)Evaluate(expr.Exprs, scope);
        scope.ResetAllocationMode();

        foreach (var exp in exprs.Exprs)
        {
            var identifier = exp as IdentifierExpression;
            if (identifier != null && !scope.Define(identifier.Name, identifier))
            {
                ErrorReporter.AddError(identifier, "duplicate definition");
            }
        }

        return new VarExpression(exprs);
    }
}
