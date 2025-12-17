#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class EcmaScriptParser : LLParser
{
    public EcmaScriptParser(ITokenReader tokenReader)
        : base(tokenReader)
    {
        RegisterParselet(Token.Identifier, new IdentifierParselet());
        RegisterParselet(Token.This, new IdentifierParselet());
        RegisterParselet(Token.NumberLiteral, new ConstantParselet());
        RegisterParselet(Token.StringLiteral, new ConstantParselet());
        RegisterParselet(Token.True, new ConstantParselet());
        RegisterParselet(Token.False, new ConstantParselet());
        RegisterParselet(Token.Null, new ConstantParselet());
        RegisterParselet(Token.RegEx, new ConstantParselet());

        RegisterParselet(Token.Equal, new AssignParselet());
        RegisterParselet(Token.AsteriskEqual, new AssignParselet());
        RegisterParselet(Token.CarotEqual, new AssignParselet());
        RegisterParselet(Token.SlashEqual, new AssignParselet());
        RegisterParselet(Token.AmpersandEqual, new AssignParselet());
        RegisterParselet(Token.PipeEqual, new AssignParselet());
        RegisterParselet(Token.PlusEqual, new AssignParselet());
        RegisterParselet(Token.PercentEqual, new AssignParselet());
        RegisterParselet(Token.MinusEqual, new AssignParselet());
        RegisterParselet(Token.LessThan2Equal, new AssignParselet());
        RegisterParselet(Token.GreaterThan2Equal, new AssignParselet());
        RegisterParselet(Token.GreaterThan3Equal, new AssignParselet());

        RegisterParselet(Token.Question, new ConditionalParselet());

        RegisterInfixLeft(Token.Pipe2, PrecedenceConstant.LogicalOr);
        RegisterInfixLeft(Token.Ampersand2, PrecedenceConstant.LogicalAnd);
        RegisterInfixLeft(Token.Pipe, PrecedenceConstant.BitwiseOr);
        RegisterInfixLeft(Token.Carot, PrecedenceConstant.BitwiseXor);
        RegisterInfixLeft(Token.Ampersand, PrecedenceConstant.BitwiseAnd);

        RegisterInfixLeft(Token.Equal2, PrecedenceConstant.Equality);
        RegisterInfixLeft(Token.Equal3, PrecedenceConstant.Equality);
        RegisterInfixLeft(Token.ExclamationEqual, PrecedenceConstant.Equality);
        RegisterInfixLeft(Token.ExclamationEqual2, PrecedenceConstant.Equality);

        RegisterInfixLeft(Token.Instanceof, PrecedenceConstant.Relational);
        RegisterInfixLeft(Token.In, PrecedenceConstant.Relational);
        RegisterInfixLeft(Token.GreaterThan, PrecedenceConstant.Relational);
        RegisterInfixLeft(Token.GreaterThanEqual, PrecedenceConstant.Relational);
        RegisterInfixLeft(Token.LessThan, PrecedenceConstant.Relational);
        RegisterInfixLeft(Token.LessThanEqual, PrecedenceConstant.Relational);

        RegisterInfixRight(Token.LessThan2, PrecedenceConstant.BitwiseShift);
        RegisterInfixRight(Token.GreaterThan2, PrecedenceConstant.BitwiseShift);
        RegisterInfixRight(Token.GreaterThan3, PrecedenceConstant.BitwiseShift);

        RegisterInfixLeft(Token.Plus, PrecedenceConstant.Sum);
        RegisterInfixLeft(Token.Minus, PrecedenceConstant.Sum);

        RegisterInfixLeft(Token.Asterisk, PrecedenceConstant.Product);
        RegisterInfixLeft(Token.Slash, PrecedenceConstant.Product);
        RegisterInfixLeft(Token.Percent, PrecedenceConstant.Product);

        RegisterPrefix(Token.Plus, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Plus2, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Minus, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Minus2, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Tilde, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Exclamation, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Delete, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Typeof, PrecedenceConstant.Prefix);
        RegisterPrefix(Token.Void, PrecedenceConstant.Prefix);

        RegisterPostfix(Token.Plus2, PrecedenceConstant.Postfix);
        RegisterPostfix(Token.Minus2, PrecedenceConstant.Postfix);

        RegisterParselet(Token.New, new NewParselet());
        RegisterParselet(Token.LeftSquareBracket, new AccessorParselet());
        RegisterParselet(Token.Point, new IdentifierPartParselet());

        RegisterParselet(Token.LeftBracket, new GroupParselet());
        RegisterParselet(Token.LeftBracket, new CallParselet());

        RegisterParselet(Token.LeftBrace, new ObjectLiteralParselet());
        RegisterParselet(Token.LeftSquareBracket, new ArrayLiteralParselet());
        RegisterParselet(Token.Function, new FunctionParselet());
    }

    public IEnumerable<IExpression> ParseModule()
    {
        var exprs = new List<IExpression>();

        while (!LookAhead(Token.EOF))
        {
            exprs.Add(ParseStatement());
            Match(Token.Semicolon);
        }
        return exprs;
    }

    private IExpression ParseStatement()
    {
        if (Match(Token.Var))
        {
            return ParseVar();
        }

        if (Match(Token.If))
        {
            return ParseIf();
        }

        if (Match(Token.Throw))
        {
            return ParseThrow();
        }

        if (Match(Token.Return))
        {
            return ParseReturn();
        }

        if (Match(Token.While))
        {
            return ParseWhile();
        }

        if (Match(Token.Do))
        {
            return ParseDoWhile();
        }

        if (Match(Token.Break))
        {
            return ParseBreak();
        }

        if (Match(Token.Continue))
        {
            return ParseContinue();
        }

        if (Match(Token.For))
        {
            return ParseFor();
        }

        if (Match(Token.Try))
        {
            return ParseTry();
        }

        if (Match(Token.Switch))
        {
            return ParseSwitch();
        }

        return ParseExpression();
    }

    public override IExpression ParseBlock()
    {
        Consume(Token.LeftBrace);

        var exprs = new List<IExpression>();
        while (!Match(Token.RightBrace))
        {
            exprs.Add(ParseStatement());
            Match(Token.Semicolon);
        }

        return new SequenceExpression(exprs);
    }

    private IExpression ParseVar()
    {
        var exprs = new List<IExpression>();
        do
        {
            exprs.Add(ParseExpression());
        } while (Match(Token.Comma));

        return new VarExpression(new SequenceExpression(exprs));
    }

    private IExpression ParseIf()
    {
        Consume(Token.LeftBracket);
        var conditionalExpr = ParseExpression();
        Consume(Token.RightBracket);
        var expr = ParseStatementOrBlock();
        IExpression elseExpr = null;
        if (LookAhead(Token.Else))
        {
            Consume(Token.Else);
            elseExpr = ParseStatementOrBlock();
        }

        return new IfExpression(conditionalExpr, expr, elseExpr);
    }

    private IExpression ParseStatementOrBlock()
    {
        var expr = LookAhead(Token.LeftBrace) ? ParseBlock() : ParseStatement();

        return expr;
    }

    private IExpression ParseThrow()
    {
        return new ThrowExpression(ParseExpression());
    }

    private IExpression ParseReturn()
    {
        var expr = LookAhead(Token.Semicolon) ? null : ParseExpression();
        return new ReturnExpression(expr);
    }

    private IExpression ParseWhile()
    {
        Consume(Token.LeftBracket);
        var conditionalExpr = ParseExpression();
        Consume(Token.RightBracket);
        var expr = ParseStatementOrBlock();

        return new ConditionalLoopExpression(conditionalExpr, expr);
    }

    private IExpression ParseDoWhile()
    {
        var expr = ParseStatementOrBlock();
        Consume(Token.While);
        Consume(Token.LeftBracket);
        var conditionalExpr = ParseExpression();
        Consume(Token.RightBracket);

        return new ConditionalLoopExpression(conditionalExpr, expr, true);
    }

    private IExpression ParseBreak()
    {
        return new BreakExpression();
    }

    private IExpression ParseContinue()
    {
        return new ContinueExpression();
    }

    private IExpression ParseFor()
    {
        var initializationExprs = new List<IExpression>();
        IExpression objectExpr = null;
        IExpression conditionExpr = null;
        IExpression incrementExpr = null;
        var isForInStatement = false;

        Consume(Token.LeftBracket);
        if (
            LookAhead(Token.Identifier, Token.In)
            || LookAhead(Token.Var, Token.Identifier, Token.In)
        )
        {
            objectExpr = LookAhead(Token.Var) ? ParseStatement() : ParseExpression();
            isForInStatement = true;
        }
        else
        {
            do
            {
                if (LookAhead(Token.Comma))
                {
                    Consume(Token.Comma);
                }

                if (!LookAhead(Token.Semicolon))
                {
                    initializationExprs.Add(
                        LookAhead(Token.Var) ? ParseStatement() : ParseExpression()
                    );
                }
            } while (LookAhead(Token.Comma));
            Consume(Token.Semicolon);

            if (!LookAhead(Token.Semicolon))
            {
                conditionExpr = ParseExpression();
            }
            Consume(Token.Semicolon);

            if (!LookAhead(Token.RightBracket))
            {
                incrementExpr = ParseExpression();
            }
        }

        Consume(Token.RightBracket);

        var expr = ParseStatementOrBlock();

        return isForInStatement
            ? (IExpression)new ForInExpression(objectExpr, expr)
            : new ForExpression(initializationExprs, conditionExpr, incrementExpr, expr);
    }

    private IExpression ParseTry()
    {
        var tryExprs = ParseBlock();
        Consume(Token.Catch);
        Consume(Token.LeftBracket);
        var catchVariable = ParseExpression();
        Consume(Token.RightBracket);
        var catchExprs = ParseBlock();

        IExpression finallyExprs = null;
        if (LookAhead(Token.Finally))
        {
            Consume(Token.Finally);
            finallyExprs = ParseBlock();
        }

        return new TryCatchFinallyExpression(tryExprs, catchVariable, catchExprs, finallyExprs);
    }

    private IExpression ParseSwitch()
    {
        Consume(Token.LeftBracket);
        var switchExprs = ParseExpression();
        Consume(Token.RightBracket);
        Consume(Token.LeftBrace);

        var caseExprs = new List<IExpression>();
        while (LookAhead(Token.Case) || LookAhead(Token.Default))
        {
            var isDefault = false;
            IExpression caseExpr = null;

            if (LookAhead(Token.Default))
            {
                Consume(Token.Default);
                isDefault = true;
            }
            else
            {
                Consume(Token.Case);
                caseExpr = ParseExpression();
            }
            Consume(Token.Colon);

            var caseStmt = new List<IExpression>();
            while (
                !(LookAhead(Token.Case) || LookAhead(Token.Default) || LookAhead(Token.RightBrace))
            )
            {
                caseStmt.Add(ParseStatement());
                Consume(Token.Semicolon);
            }

            caseExprs.Add(
                new CaseExpression(caseExpr, new SequenceExpression(caseStmt), isDefault)
            );
        }
        Consume(Token.RightBrace);

        return new SwitchExpression(switchExprs, new SequenceExpression(caseExprs));
    }
}
