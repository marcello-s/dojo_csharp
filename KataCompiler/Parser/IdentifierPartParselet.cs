#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class IdentifierPartParselet : IInfixParselet
{
    private const string ErrorMessage =
        "The right-hand side of the expression must be an IdentifierExpression or a CallExpression but was ";
    public int Precedence { get; private set; }

    public IdentifierPartParselet()
    {
        Precedence = PrecedenceConstant.Accessor;
    }

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var expr = parser.ParseExpression(Precedence);
        if (!(expr is IdentifierExpression || expr is CallExpression))
        {
            parser.ErrorReporter.AddError(token, ErrorMessage + expr.GetType().Name);
            return new IllegalExpression(left, expr, ErrorMessage + expr.GetType().Name, token);
        }

        return new IdentifierPartExpression(left, expr);
    }
}
