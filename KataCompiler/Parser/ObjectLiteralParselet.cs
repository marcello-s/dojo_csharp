#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class ObjectLiteralParselet : IPrefixParselet
{
    private const string ErrorMessage =
        "The left-hand side of the semicolon must be an IdentifierExpression or a ConstantExpression but was ";

    public IExpression Parse(LLParser parser, TokenValue token)
    {
        var definitions = new List<IExpression>();

        while (!parser.LookAhead(Token.RightBrace))
        {
            while (true)
            {
                var identifierExpr = parser.ParseExpression();
                if (
                    !(
                        identifierExpr is IdentifierExpression
                        || identifierExpr is ConstantExpression
                    )
                )
                {
                    parser.ErrorReporter.AddError(
                        token,
                        ErrorMessage + identifierExpr.GetType().Name
                    );
                    return new IllegalExpression(
                        null,
                        identifierExpr,
                        ErrorMessage + identifierExpr.GetType().Name,
                        token
                    );
                }

                parser.Consume(Token.Colon);

                var definitionExpr = parser.ParseExpression();
                definitions.Add(new DefinitionExpression(identifierExpr, definitionExpr));

                if (parser.LookAhead(Token.RightBrace))
                {
                    break;
                }

                parser.Consume(Token.Comma);
            }
        }

        parser.Consume(Token.RightBrace);

        return new ObjectLiteralExpression(new SequenceExpression(definitions));
    }
}
