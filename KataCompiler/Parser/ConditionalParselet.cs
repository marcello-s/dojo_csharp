#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class ConditionalParselet : IInfixParselet
{
    public int Precedence { get; private set; }

    public ConditionalParselet()
    {
        Precedence = PrecedenceConstant.Conditional;
    }

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var truthyBranch = parser.ParseExpression();
        parser.Consume(Token.Colon);
        var falsyBranch = parser.ParseExpression();
        return new ConditionalExpression(left, truthyBranch, falsyBranch);
    }
}
