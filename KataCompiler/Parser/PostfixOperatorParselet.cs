#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class PostfixOperatorParselet(int precedence) : IInfixParselet
{
    public int Precedence { get; private set; } = precedence;

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        return new PostfixExpression(left, token.TokenId);
    }
}
