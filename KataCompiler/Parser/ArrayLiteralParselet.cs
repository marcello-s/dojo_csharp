#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class ArrayLiteralParselet : IPrefixParselet
{
    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var values = new List<IExpression>();

        while (!parser.LookAhead(Token.RightSquareBracket))
        {
            var value = parser.ParseExpression();
            values.Add(value);

            if (parser.LookAhead(Token.RightSquareBracket))
            {
                break;
            }

            parser.Consume(Token.Comma);
        }

        parser.Consume(Token.RightSquareBracket);

        return new ArrayLiteralExpression(new SequenceExpression(values));
    }
}
