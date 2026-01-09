#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class AssignParselet : IInfixParselet
{
    public int Precedence { get; private set; }

    public AssignParselet()
    {
        Precedence = PrecedenceConstant.Assignment;
    }

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var right = parser.ParseExpression(PrecedenceConstant.Assignment - 1);

        if (
            !(
                left is IdentifierExpression
                || left is IdentifierPartExpression
                || left is AccessorExpression
            )
        )
        {
            throw new Exception(
                "The left-hand side of an assignment must be an IdentifierExpression, IdentifierPartExpression or AccessorExpression but was "
                    + left.GetType().Name
            );
        }

        return new AssignExpression(left, right);
    }
}
