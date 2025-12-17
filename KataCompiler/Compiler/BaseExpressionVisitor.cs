#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Compiler;

abstract class BaseExpressionVisitor : IExpressionVisitor<IExpression, Scope>
{
    public virtual IExpression Evaluate(IExpression expr, Scope scope)
    {
        return expr == null ? null : expr.Accept(this, scope);
    }

    public virtual IExpression Visit(AccessorExpression expr, Scope scope)
    {
        var member = Evaluate(expr.Member, scope);
        var accessorExpr = Evaluate(expr.AccessorExpr, scope);

        return new AccessorExpression(member, accessorExpr);
    }

    public virtual IExpression Visit(ArrayLiteralExpression expr, Scope scope)
    {
        var values = Evaluate(expr.Values, scope);

        return new ArrayLiteralExpression(values);
    }

    public virtual IExpression Visit(AssignExpression expr, Scope scope)
    {
        var left = Evaluate(expr.Left, scope);
        var right = Evaluate(expr.Right, scope);
        var assignExpr = new AssignExpression(left, right) { Tag = expr.Tag };

        return assignExpr;
    }

    public virtual IExpression Visit(BinaryOperatorExpression expr, Scope scope)
    {
        var left = Evaluate(expr.Left, scope);
        var right = Evaluate(expr.Right, scope);

        return new BinaryOperatorExpression(left, expr.TokenId, right);
    }

    public virtual IExpression Visit(BreakExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(CallExpression expr, Scope scope)
    {
        var function = Evaluate(expr.Function, scope);
        var args = Evaluate(expr.Args, scope);

        return new CallExpression(function, args);
    }

    public virtual IExpression Visit(CaseExpression expr, Scope scope)
    {
        var caseExpr = Evaluate(expr.CaseExpr, scope);
        var stmtExpr = Evaluate(expr.StmtExpr, scope);

        return new CaseExpression(caseExpr, stmtExpr, expr.IsDefault);
    }

    public virtual IExpression Visit(ConditionalExpression expr, Scope scope)
    {
        var condition = Evaluate(expr.Condition, scope);
        var truthyBranch = Evaluate(expr.TruthyBranch, scope);
        var falsyBranch = Evaluate(expr.FalsyBranch, scope);

        return new ConditionalExpression(condition, truthyBranch, falsyBranch);
    }

    public virtual IExpression Visit(ConditionalLoopExpression expr, Scope scope)
    {
        var conditionalExpr = Evaluate(expr.ConditionalExpr, scope);
        var body = Evaluate(expr.Expr, scope);

        return new ConditionalLoopExpression(conditionalExpr, body, expr.PostEvaluation);
    }

    public virtual IExpression Visit(ConstantExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(ContinueExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(DefinitionExpression expr, Scope scope)
    {
        var definition = Evaluate(expr.DefinitionExpr, scope);

        return new DefinitionExpression(expr.IdentifierExpr, definition);
    }

    public virtual IExpression Visit(ForExpression expr, Scope scope)
    {
        var initExprs = new List<IExpression>();
        foreach (var initExpr in expr.InitExprs)
        {
            initExprs.Add(Evaluate(initExpr, scope));
        }

        var conditionExpr = Evaluate(expr.ConditionExpr, scope);
        var incrementExpr = Evaluate(expr.IncrementExpr, scope);
        var body = Evaluate(expr.Expr, scope);

        return new ForExpression(initExprs, conditionExpr, incrementExpr, body);
    }

    public virtual IExpression Visit(ForInExpression expr, Scope scope)
    {
        var objectExpr = Evaluate(expr.ObjectExpr, scope);
        var body = Evaluate(expr.Expr, scope);

        return new ForInExpression(objectExpr, body);
    }

    public virtual IExpression Visit(IdentifierExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(IdentifierPartExpression expr, Scope scope)
    {
        var left = Evaluate(expr.Left, scope);
        var exprs = Evaluate(expr.Exprs, scope);

        return new IdentifierPartExpression(left, exprs);
    }

    public virtual IExpression Visit(IfExpression expr, Scope scope)
    {
        var conditionalExpr = Evaluate(expr.ConditionalExpr, scope);
        var exp = Evaluate(expr.Expr, scope);
        var elseExpr = Evaluate(expr.ElseExpr, scope);

        return new IfExpression(conditionalExpr, exp, elseExpr);
    }

    public virtual IExpression Visit(IllegalExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(MethodExpression expr, Scope scope)
    {
        var args = (SequenceExpression)Evaluate(expr.Args, scope);
        var body = Evaluate(expr.Body, scope);

        return new MethodExpression(expr.Name, args, body);
    }

    public virtual IExpression Visit(NewExpression expr, Scope scope)
    {
        var callExpr = Evaluate(expr.CallExpr, scope);

        return new NewExpression(callExpr);
    }

    public virtual IExpression Visit(ObjectLiteralExpression expr, Scope scope)
    {
        var definitions = Evaluate(expr.Definitions, scope);

        return new ObjectLiteralExpression(definitions);
    }

    public virtual IExpression Visit(PostfixExpression expr, Scope scope)
    {
        return expr;
    }

    public virtual IExpression Visit(PrefixExpression expr, Scope scope)
    {
        var right = Evaluate(expr.Right, scope);

        return new PrefixExpression(expr.TokenId, right);
    }

    public virtual IExpression Visit(ReturnExpression expr, Scope scope)
    {
        var exprs = Evaluate(expr.Expr, scope);

        return new ReturnExpression(exprs);
    }

    public virtual IExpression Visit(SequenceExpression expr, Scope scope)
    {
        var exprs = new List<IExpression>();

        foreach (var exp in expr.Exprs)
        {
            exprs.Add(Evaluate(exp, scope));
        }

        return new SequenceExpression(exprs);
    }

    public virtual IExpression Visit(SwitchExpression expr, Scope scope)
    {
        var switchExpr = Evaluate(expr.SwitchExpr, scope);
        var caseExpr = Evaluate(expr.CaseExpr, scope);

        return new SwitchExpression(switchExpr, caseExpr);
    }

    public virtual IExpression Visit(ThrowExpression expr, Scope scope)
    {
        var exprs = Evaluate(expr.Expr, scope);

        return new ThrowExpression(exprs);
    }

    public virtual IExpression Visit(TryCatchFinallyExpression expr, Scope scope)
    {
        var tryExprs = Evaluate(expr.TryExprs, scope);
        var catchVariable = Evaluate(expr.CatchVariable, scope);
        var catchExprs = Evaluate(expr.CatchExprs, scope);
        var finallyExprs = Evaluate(expr.FinallyExprs, scope);

        return new TryCatchFinallyExpression(tryExprs, catchVariable, catchExprs, finallyExprs);
    }

    public virtual IExpression Visit(VarExpression expr, Scope scope)
    {
        var exprs = Evaluate(expr.Exprs, scope);

        return new VarExpression(exprs);
    }
}
