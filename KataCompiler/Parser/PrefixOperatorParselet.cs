#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class PrefixOperatorParselet : IPrefixParselet
{
    public int Precedence { get; private set; }

    public PrefixOperatorParselet(int precedence)
    {
        Precedence = precedence;
    }

    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var right = parser.ParseExpression(Precedence);
        return new PrefixExpression(token.TokenId, right);
    }
}
