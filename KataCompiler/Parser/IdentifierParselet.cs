#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class IdentifierParselet : IPrefixParselet
{
    public IExpression Parse(LLParser parser, TokenValue token)
    {
        return new IdentifierExpression(token.Literal);
    }
}
