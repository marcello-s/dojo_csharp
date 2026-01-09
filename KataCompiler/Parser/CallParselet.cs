#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class CallParselet : IInfixParselet
{
    public int Precedence { get; private set; }

    public CallParselet()
    {
        Precedence = PrecedenceConstant.Call;
    }

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var args = new List<IExpression>();

        if (!parser.Match(Token.RightBracket))
        {
            do
            {
                args.Add(parser.ParseExpression());
            } while (parser.Match(Token.Comma));
            parser.Consume(Token.RightBracket);
        }

        return new CallExpression(left, new SequenceExpression(args));
    }
}
