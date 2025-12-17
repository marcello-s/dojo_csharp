#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class ConstantParselet : IPrefixParselet
{
    private static readonly IDictionary<Token, ConstantType> Map = new Dictionary<
        Token,
        ConstantType
    >
    {
        { Token.NumberLiteral, ConstantType.Number },
        { Token.StringLiteral, ConstantType.String },
        { Token.True, ConstantType.Boolean },
        { Token.False, ConstantType.Boolean },
        { Token.Null, ConstantType.Null },
        { Token.RegEx, ConstantType.RegEx },
    };

    public IExpression Parse(LLParser parser, TokenValue token)
    {
        return new ConstantExpression(token.Literal, Map[token.TokenId]);
    }
}
