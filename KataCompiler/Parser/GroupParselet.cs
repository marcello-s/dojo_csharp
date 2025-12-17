#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class GroupParselet : IPrefixParselet
{
    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var expr = parser.ParseExpression();
        parser.Consume(Token.RightBracket);
        return expr;
    }
}
