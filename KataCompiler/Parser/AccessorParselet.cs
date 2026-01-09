#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class AccessorParselet : IInfixParselet
{
    public int Precedence { get; private set; }

    public AccessorParselet()
    {
        Precedence = PrecedenceConstant.Accessor;
    }

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var expr = parser.ParseExpression();
        parser.Consume(Token.RightSquareBracket);

        return new AccessorExpression(left, expr);
    }
}
