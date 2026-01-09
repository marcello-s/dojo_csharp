#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class PrefixOperatorParselet(int precedence) : IPrefixParselet
{
    public int Precedence { get; private set; } = precedence;

    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var right = parser.ParseExpression(Precedence);
        return new PrefixExpression(token.TokenId, right);
    }
}
