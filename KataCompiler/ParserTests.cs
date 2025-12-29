#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCompiler;

[TestFixture]
public class ParserTests
{
    [Test]
    public void Parse_WhenCalled_ReturnExpression()
    {
        // (4.0+5.0)*7.0
        var tokens = new List<TokenValue>
        {
            new TokenValue { TokenId = Token.LeftBracket },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "4.0" },
            new TokenValue { TokenId = Token.Plus },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "5.0" },
            new TokenValue { TokenId = Token.RightBracket },
            new TokenValue { TokenId = Token.Asterisk },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "7.0" },
        };

        var parser = new ExpressionParser();
        var expr = parser.Parse(tokens);
        var value = expr?.Evaluate();
        Console.WriteLine("value:{0}", value);
        PrintExpression(expr!, 0);

        //      *
        //     / \
        //    +   7
        //   / \
        //  4   5

        Assert.That(value, Is.EqualTo(63));
    }

    [Test]
    public void Parse_WhenStreamContainsPower_ReturnExpression()
    {
        // 4.0+5.0*7.0^2.0
        var tokens = new List<TokenValue>
        {
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "4.0" },
            new TokenValue { TokenId = Token.Plus },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "5.0" },
            new TokenValue { TokenId = Token.Asterisk },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "7.0" },
            new TokenValue { TokenId = Token.Carot },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "2.0" },
        };

        var parser = new ExpressionParser();
        var expr = parser.Parse(tokens);
        var value = expr?.Evaluate();
        Console.WriteLine("value:{0}", value);
        PrintExpression(expr!, 0);

        //       +
        //      / \
        //     4   *
        //        / \
        //       5   ^
        //          / \
        //         7   2

        Assert.That(value, Is.EqualTo(249));
    }

    [Test]
    public void Parse_WhenStreamContainsUnaryMinus_ReturnExpression()
    {
        // 4.0+5.0*7.0^-2.0
        var tokens = new List<TokenValue>
        {
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "4.0" },
            new TokenValue { TokenId = Token.Plus },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "5.0" },
            new TokenValue { TokenId = Token.Asterisk },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "7.0" },
            new TokenValue { TokenId = Token.Carot },
            new TokenValue { TokenId = Token.Minus },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "2.0" },
        };

        var parser = new ExpressionParser();
        var expr = parser.Parse(tokens);
        var value = expr?.Evaluate() ?? 0;
        Console.WriteLine("value:{0}", value);
        PrintExpression(expr!, 0);

        //        +
        //       / \
        //      4   *
        //         / \
        //        5   ^
        //           / \
        //          7   *
        //             / \
        //           -1   2

        Assert.That(Math.Round(value, 5), Is.EqualTo(4.10204));
    }

    private static void PrintExpression(Expression expr, int level)
    {
        var indent = new string('.', level);
        if (expr is BinaryOperator)
        {
            var op = expr as BinaryOperator;
            Console.WriteLine(indent + op!.OpCode);
            PrintExpression(op.Left, level + 1);
            PrintExpression(op.Right, level + 1);
        }

        if (expr is Constant)
        {
            var c = expr as Constant;
            Console.WriteLine(indent + c!.Value);
        }
    }
}

public class ExpressionParser
{
    // E = T Eopt .
    // Eopt = "+" T Eopt | "-" T Eopt | .
    // T = F Topt .
    // Topt = "*" F Topt | "/" F Topt | .
    // F = Real | "(" E ")" .

    // E = T Eopt .
    // Eopt = "+" T Eopt | "-" T Eopt | .
    // T = F Topt .
    // Topt = "*" P Topt | "/" P Topt | .
    // P = F Popt .
    // Popt = "^" F P .
    // F = Real | "(" E ")" | "-" T .

    // E --> T {("+"|"-") T}
    // T --> F {("*"|"/") F}
    // F --> P ["^" F]
    // P --> v | "(" E ")" | "-" T

    public Expression? Parse(IEnumerable<TokenValue> tokens)
    {
        var stream = tokens.GetEnumerator();
        stream.MoveNext();
        return E(stream);
    }

    private Expression? E(IEnumerator<TokenValue> stream)
    {
        return Eopt(T(stream), stream);
    }

    private Expression? T(IEnumerator<TokenValue> stream)
    {
        return Topt(F(stream), stream);
    }

    private Expression? Eopt(Expression? expr, IEnumerator<TokenValue> stream)
    {
        switch (stream.Current.TokenId)
        {
            case Token.Plus:
                stream.MoveNext();
                var tExpr = T(stream);
                if (expr != null && tExpr != null)
                {
                    expr = Eopt(new BinaryOperator(expr, tExpr, "+"), stream);
                }
                break;

            case Token.Minus:
                stream.MoveNext();
                var ttExpr = T(stream);
                if (expr != null && ttExpr != null)
                {
                    expr = Eopt(new BinaryOperator(expr, ttExpr, "-"), stream);
                }
                break;

            default:
                break;
        }

        return expr;
    }

    private Expression? F(IEnumerator<TokenValue> stream)
    {
        Expression? e = null;
        switch (stream.Current.TokenId)
        {
            case Token.NumberLiteral:
                e = new Constant(double.Parse(stream.Current.Literal));
                stream.MoveNext();
                break;

            case Token.LeftBracket:
                stream.MoveNext();
                e = E(stream);
                if (stream.Current.TokenId != Token.RightBracket)
                {
                    throw new ApplicationException("parse error: ')' expected");
                }
                stream.MoveNext();
                break;

            case Token.Minus:
                stream.MoveNext();
                var fExpr = F(stream);
                if (fExpr != null)
                {
                    e = new BinaryOperator(new Constant(-1d), fExpr, "*");
                }
                break;

            default:
                break;
        }

        return e;
    }

    private Expression? Topt(Expression? expr, IEnumerator<TokenValue> stream)
    {
        switch (stream.Current.TokenId)
        {
            case Token.Asterisk:
                stream.MoveNext();
                var pExpr = P(stream);
                if (expr != null && pExpr != null)
                {
                    expr = Topt(new BinaryOperator(expr, pExpr, "*"), stream);
                }
                break;

            case Token.Slash:
                stream.MoveNext();
                var ppExpr = P(stream);
                if (expr != null && ppExpr != null)
                {
                    expr = Topt(new BinaryOperator(expr, ppExpr, "/"), stream);
                }
                break;

            default:
                break;
        }

        return expr;
    }

    private Expression? P(IEnumerator<TokenValue> stream)
    {
        return Popt(F(stream), stream);
    }

    private Expression? Popt(Expression? expr, IEnumerator<TokenValue> stream)
    {
        switch (stream.Current.TokenId)
        {
            case Token.Carot:
                stream.MoveNext();
                var fExpr = F(stream);
                if (expr != null && fExpr != null)
                {
                    expr = Popt(new BinaryOperator(expr, fExpr, "^"), stream);
                }
                break;

            default:
                break;
        }

        return expr;
    }
}

public abstract class Expression
{
    public abstract double Evaluate();
}

class BinaryOperator : Expression
{
    public Expression Left { get; private set; }
    public Expression Right { get; private set; }
    public string OpCode { get; private set; }

    public BinaryOperator(Expression left, Expression right, string opCode)
    {
        Left = left;
        Right = right;
        OpCode = opCode;
    }

    public override double Evaluate()
    {
        var value = 0d;
        switch (OpCode)
        {
            case "+":
                value = Left.Evaluate() + Right.Evaluate();
                break;

            case "-":
                value = Left.Evaluate() - Right.Evaluate();
                break;

            case "*":
                value = Left.Evaluate() * Right.Evaluate();
                break;

            case "/":
                value = Left.Evaluate() / Right.Evaluate();
                break;
            case "^":
                value = Math.Pow(Left.Evaluate(), Right.Evaluate());
                break;

            default:
                break;
        }

        return value;
    }
}

class Constant : Expression
{
    public double Value { get; private set; }

    public Constant(double value)
    {
        Value = value;
    }

    public override double Evaluate()
    {
        return Value;
    }
}
