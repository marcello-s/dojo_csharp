#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Parser;

class Morpher : ITokenReader
{
    private readonly ITokenReader _tokenReader;

    public Morpher(ITokenReader tokenReader)
    {
        _tokenReader = tokenReader;
    }

    public TokenValue ReadToken()
    {
        while (true)
        {
            var token = _tokenReader.ReadToken();

            switch (token.TokenId)
            {
                case Token.LineComment:
                case Token.BlockComment:
                case Token.NewLine:
                case Token.WhiteSpace:
                    continue;

                default:
                    break;
            }

            return token;
        }
    }
}
