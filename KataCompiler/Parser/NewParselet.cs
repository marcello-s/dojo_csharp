#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class NewParselet : IPrefixParselet
{
    private const string ErrorMessage =
        "The right-hand side of new must be a CallExpression but was ";
    public int Precedence { get; private set; }

    public NewParselet()
    {
        Precedence = PrecedenceConstant.New;
    }

    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var callExpr = parser.ParseExpression();
        if (!(callExpr is CallExpression))
        {
            parser.ErrorReporter.AddError(token, ErrorMessage + callExpr.GetType().Name);
            return new IllegalExpression(
                null,
                callExpr,
                ErrorMessage + callExpr.GetType().Name,
                token
            );
        }

        return new NewExpression(callExpr);
    }
}
