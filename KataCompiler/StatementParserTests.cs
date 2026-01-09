#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCompiler;

[TestFixture]
public class StatementParserTests
{
    [Test]
    public void Parse_WhenCalled_ReturnStatement()
    {
        // if(true)a=1;
        var tokens = new List<TokenValue>
        {
            new TokenValue { TokenId = Token.If },
            new TokenValue { TokenId = Token.LeftBracket },
            new TokenValue { TokenId = Token.True },
            new TokenValue { TokenId = Token.RightBracket },
            new TokenValue { TokenId = Token.Identifier, Literal = "a" },
            new TokenValue { TokenId = Token.Equal },
            new TokenValue { TokenId = Token.NumberLiteral, Literal = "1" },
            new TokenValue { TokenId = Token.Semicolon },
        };

        var parser = new StatementParser();
        var stat = parser.Parse(tokens);
    }
}

public class StatementParser
{
    public Statement? Parse(IEnumerable<TokenValue> tokens)
    {
        // block
        // { [<block|statement [statement]>] }

        // conditional statement
        // if ( <expression> ) <statement|block> [else <statement|block>]

        var stream = tokens.GetEnumerator();
        stream.MoveNext();
        return ParseStatement(stream);
    }

    private Statement? ParseStatement(IEnumerator<TokenValue> stream)
    {
        Statement? s = null;
        switch (stream.Current.TokenId)
        {
            case Token.If:
                s = ParseIfStatement(stream);
                break;

            case Token.LeftBrace:
                //s = ParseBlockStatement(stream);
                break;

            default:
                // most likely an assignment to parse up to the semicolon
                s = new Statement();
                break;
        }

        return s;
    }

    private IfStatement ParseIfStatement(IEnumerator<TokenValue> stream)
    {
        stream.MoveNext();
        if (!stream.Current.TokenId.Equals(Token.LeftBracket))
        {
            throw new Exception("'(' expected");
        }

        var condition = ParseExpression(stream);
        if (!stream.Current.TokenId.Equals(Token.RightBracket))
        {
            throw new Exception("')' expected");
        }

        var conditionStatement = ParseStatement(stream);
        return new IfStatement(condition, conditionStatement!, null!);
    }

    private BooleanExpression ParseExpression(IEnumerator<TokenValue> stream)
    {
        stream.MoveNext();
        stream.MoveNext();
        return new BooleanConstant(true);
    }
}

public abstract class BooleanExpression
{
    public abstract bool Evaluate();
}

public class BooleanConstant : BooleanExpression
{
    public bool Value { get; private set; }

    public BooleanConstant(bool value)
    {
        Value = value;
    }

    public override bool Evaluate()
    {
        return Value;
    }
}

public class Statement { }

public class IfStatement : Statement
{
    public BooleanExpression Condition { get; private set; }
    public Statement ConditionStatement { get; private set; }
    public Statement ElseStatement { get; private set; }

    public IfStatement(
        BooleanExpression condition,
        Statement conditionStatement,
        Statement elseStatement
    )
    {
        Condition = condition;
        ConditionStatement = conditionStatement;
        ElseStatement = elseStatement;
    }
}
