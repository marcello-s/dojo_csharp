#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Parser;

class Lexer : ITokenReader
{
    private readonly TextReader textReader;
    private int line,
        column;
    private int startLine,
        startColumn;
    private readonly char[] buffer = new char[1];
    private readonly StringBuilder text;
    private Token last;
    private const string NumberPunctuation = ".-+abcdefx";
    private const string HexNumberPunctuation = "abcdef";

    public Lexer(TextReader textReader)
    {
        this.textReader = textReader;
        text = new StringBuilder();
    }

    public TokenValue ReadToken()
    {
        var c = Advance();

        TokenValue token;

        switch (c)
        {
            case ' ':
            case '\t':
                token = ReadWhiteSpace();
                break;

            case '\n':
            case '\r':
                token = CreateToken(Token.NewLine);
                break;

            case '\'':
            case '"':
                token = ReadString();
                break;

            case '/':
                switch (Peek())
                {
                    case '/':
                        token = ReadLineComment();
                        break;
                    case '*':
                        token = ReadBlockComment();
                        break;

                    default:
                        if (
                            last.Equals(Token.Identifier)
                            || last.Equals(Token.NumberLiteral)
                            || last.Equals(Token.RightBracket)
                        )
                        {
                            token = ReadOperator();
                        }
                        else
                        {
                            token = ReadRegEx();
                        }
                        break;
                }
                break;

            case char.MinValue:
                token = CreateToken(Token.EOF);
                break;

            default:
                if (IsName(c))
                {
                    token = ReadIdentifier();
                }
                else if (IsOperator(c))
                {
                    token = ReadOperator();
                }
                else if (IsDigit(c))
                {
                    token = ReadNumber();
                }
                else if (IsPunctuation(c))
                {
                    token = CreateToken(Punctuation.Punctuators[text.ToString()]);
                }
                else
                {
                    token = CreateToken(Token.Illegal, text.ToString(), "Unrecognized literal.");
                }
                break;
        }

        return token;
    }

    private TokenValue ReadWhiteSpace()
    {
        while (true)
        {
            switch (Peek())
            {
                case ' ':
                case '\t':
                    Advance();
                    break;

                default:
                    return CreateToken(Token.WhiteSpace);
            }
        }
    }

    private TokenValue ReadString()
    {
        var escaping = new StringBuilder();
        var singleQuoted = text.ToString().Substring(0, 1).Equals("'");

        while (true)
        {
            var c = Advance();

            switch (c)
            {
                case '\\':
                    var e = Advance();

                    switch (e)
                    {
                        case 'n':
                            escaping.Append("\n");
                            break;
                        case 'r':
                            escaping.Append("\r");
                            break;
                        case 't':
                            escaping.Append("\t");
                            break;
                        case '"':
                            escaping.Append("\"");
                            break;
                        case '\\':
                            escaping.Append("\\");
                            break;
                        case '0':
                            escaping.Append("\0");
                            break;
                    }
                    break;

                case '\'':
                    if (singleQuoted)
                    {
                        return CreateToken(Token.StringLiteral, escaping.ToString());
                    }
                    break;

                case '"':
                    if (!singleQuoted)
                    {
                        return CreateToken(Token.StringLiteral, escaping.ToString());
                    }
                    else
                    {
                        escaping.Append("\"");
                    }
                    break;

                default:
                    escaping.Append(c);
                    break;
            }
        }
    }

    private TokenValue ReadLineComment()
    {
        Advance();

        while (true)
        {
            switch (Peek())
            {
                case '\n':
                case '\r':
                case '\0':
                    return CreateToken(Token.LineComment);

                default:
                    Advance();
                    break;
            }
        }
    }

    private TokenValue ReadBlockComment()
    {
        while (true)
        {
            switch (Advance())
            {
                case '*':
                    switch (Advance())
                    {
                        case '/':
                            return CreateToken(Token.BlockComment);
                    }
                    break;

                case '\0':
                    return CreateToken(
                        Token.Illegal,
                        text.ToString(),
                        "Unterminated block comment."
                    );
            }
        }
    }

    private TokenValue ReadRegEx()
    {
        // read until an unescaped slash / and max. 3 optional letters (gim)
        // then terminate on whitespace, comma, dot or EOL .
        const string Options = "gim";

        while (true)
        {
            var c = Advance();

            // if escape '\' then skip the next character
            if (c == '\\')
            {
                Advance();
            }
            else if (c == '/')
            {
                while (true)
                {
                    var o = Peek();
                    if (char.IsWhiteSpace(o) || o == ',' || o == '.' || o == '\0')
                    {
                        return CreateToken(Token.RegEx);
                    }

                    if (Options.Contains(o))
                    {
                        Advance();
                    }
                    else
                    {
                        return CreateToken(Token.RegEx);
                    }
                }
            }
        }
    }

    private TokenValue ReadOperator()
    {
        while (true)
        {
            var p = Peek();
            if (IsOperator(p))
            {
                if ((text + p.ToString()).Equals("!!"))
                {
                    return CreateToken(Punctuation.Operators[text.ToString()]);
                }

                Advance();
            }
            else
            {
                return CreateToken(Punctuation.Operators[text.ToString()]);
            }
        }
    }

    private TokenValue ReadNumber()
    {
        var punctuation = new bool[NumberPunctuation.Length];
        var isHexNumber = false;

        while (true)
        {
            var c = Peek();
            if (
                (char.IsDigit(c) || NumberPunctuation.Contains(char.ToLower(c)))
                && !IsPunctuationUsed(c, ref punctuation)
            )
            {
                SetNumberPunctuation(c, ref punctuation, ref isHexNumber);
                Advance();
            }
            else
            {
                return CreateToken(Token.NumberLiteral);
            }
        }
    }

    private static bool IsPunctuationUsed(char c, ref bool[] punctuation)
    {
        var ix = NumberPunctuation.IndexOf(c);
        if (ix > -1)
        {
            return punctuation[ix];
        }

        return false;
    }

    private static void SetNumberPunctuation(char c, ref bool[] punctuation, ref bool isHexNumber)
    {
        if (!isHexNumber)
        {
            isHexNumber = c == 'x';
        }

        var ix = NumberPunctuation.IndexOf(c);
        if (ix > -1 && !isHexNumber)
        {
            punctuation[ix] = true;
        }
    }

    private TokenValue ReadIdentifier()
    {
        while (true)
        {
            var c = Peek();
            if (IsName(c) || IsDigit(c))
            {
                Advance();
            }
            else
            {
                var text = this.text.ToString();
                if (Punctuation.FutureReservedWord.Contains(text))
                {
                    return CreateToken(Token.Illegal, text, "This is a future reserved keyword.");
                }
                if (Punctuation.Keywords.ContainsKey(text))
                {
                    return CreateToken(Punctuation.Keywords[text]);
                }
                return CreateToken(Token.Identifier);
            }
        }
    }

    private static bool IsName(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '$' || c == '_';
    }

    private static bool IsOperator(char c)
    {
        return "~!%^&*/-=+|/?<>:".Contains(c.ToString());
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool IsPunctuation(char c)
    {
        return "(){}[];,.".Contains(c.ToString());
    }

    private char Peek()
    {
        return (char)textReader.Peek();
    }

    private char Advance()
    {
        var c = char.MinValue;
        if (textReader.Read(buffer, 0, 1) > 0)
        {
            c = buffer[0];
            text.Append(buffer);

            if (c == '\n')
            {
                line++;
                column = 0;
            }
            else
            {
                column++;
            }
        }

        return c;
    }

    private TokenValue CreateToken(Token tokenId, string literal = "", string message = "")
    {
        var position = CreatePosition();
        var text = string.IsNullOrEmpty(literal) ? position.Text : literal;

        startLine = line;
        startColumn = column;
        this.text.Clear();

        if (!tokenId.Equals(Token.WhiteSpace))
        {
            last = tokenId;
        }

        return new TokenValue
        {
            SrcPosition = position,
            TokenId = tokenId,
            Literal = text,
            Message = message,
        };
    }

    private Position CreatePosition()
    {
        return new Position(text.ToString(), startLine, line, startColumn, column);
    }
}
