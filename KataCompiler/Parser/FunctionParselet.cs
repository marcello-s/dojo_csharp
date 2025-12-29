#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class FunctionParselet : IPrefixParselet
{
    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var name = new TokenValue();
        if (parser.LookAhead(Token.Identifier))
        {
            name = parser.Consume(Token.Identifier);
        }

        var args = new List<IExpression>();
        parser.Consume(Token.LeftBracket);
        if (!parser.Match(Token.RightBracket))
        {
            do
            {
                args.Add(parser.ParseExpression());
            } while (parser.Match(Token.Comma));
            parser.Consume(Token.RightBracket);
        }

        var body = parser.ParseBlock();

        return new MethodExpression(name.Literal, new SequenceExpression(args), body!);
    }
}
