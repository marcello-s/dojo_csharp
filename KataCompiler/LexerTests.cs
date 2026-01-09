#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using NUnit.Framework;

namespace KataCompiler;

[TestFixture]
public class LexerTests
{
    [Test]
    public void Lexer_SimpleTest()
    {
        var input =
            "function Division(a, b) {"
            + Environment.NewLine
            + "  var result = 0.0e1;"
            + Environment.NewLine
            + "  if (b === 0) {"
            + Environment.NewLine
            + "    throw \"Division by zero!\";"
            + Environment.NewLine
            + "  }"
            + Environment.NewLine
            + "  else {"
            + Environment.NewLine
            + "    result = a / b;"
            + Environment.NewLine
            + "  }"
            + Environment.NewLine
            + "  return result;"
            + Environment.NewLine
            + "}";

        var bytes = Encoding.Default.GetBytes(input);
        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var lexer = new Lexer(sr);

            var token = lexer.ReadToken();
            while (token.TokenId != Token.EOF)
            {
                Console.WriteLine(FormatTokenValue(token));
                token = lexer.ReadToken();
            }
        }
    }

    [Test]
    public void Lexer_EscapedString()
    {
        const string input = "The \"quick brown fox\" jumps over the \"\\\"lazy\\\"\" dog.";

        var bytes = Encoding.Default.GetBytes(input);
        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var lexer = new Lexer(sr);

            var token = lexer.ReadToken();
            while (token.TokenId != Token.EOF)
            {
                Console.WriteLine(FormatTokenValue(token));
                token = lexer.ReadToken();
            }
        }
    }

    [Test]
    public void Lexer_Number()
    {
        const string input = "123 45e-6 78.90 12E+3 0xabcdff";

        var bytes = Encoding.Default.GetBytes(input);
        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var lexer = new Lexer(sr);

            var token = lexer.ReadToken();
            while (token.TokenId != Token.EOF)
            {
                Console.WriteLine(FormatTokenValue(token));
                token = lexer.ReadToken();
            }
        }
    }

    [Test]
    public void Lexer_Comment()
    {
        var input =
            " /* 123.5 */// -line-comment-"
            + Environment.NewLine
            + " /* -illegal"
            + Environment.NewLine
            + " ";

        var bytes = Encoding.Default.GetBytes(input);
        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var lexer = new Lexer(sr);

            var token = lexer.ReadToken();
            while (token.TokenId != Token.EOF)
            {
                Console.WriteLine(FormatTokenValue(token));
                token = lexer.ReadToken();
            }
        }
    }

    private static string FormatTokenValue(TokenValue tokenValue)
    {
        var sb = new StringBuilder();

        sb.AppendFormat("tok:'{0}' ", tokenValue.TokenId);
        //sb.AppendFormat("src:'{0}' ", tokenValue.Span.Text);

        if (!string.IsNullOrEmpty(tokenValue.Literal))
        {
            sb.AppendFormat("lit:'{0}' ", tokenValue.Literal);
        }

        if (!string.IsNullOrEmpty(tokenValue.Message))
        {
            sb.AppendFormat("msg:'{0}'", tokenValue.Message);
        }

        return sb.ToString();
    }

    class Lexer
    {
        private readonly TextReader _textReader;
        private int _line,
            _column;
        private int _startLine,
            _startColumn;
        private readonly char[] _buffer = new char[1];
        private readonly StringBuilder _text;

        public Lexer(TextReader textReader)
        {
            _textReader = textReader;
            _text = new StringBuilder();
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
                            token = ReadOperator();
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
                        token = CreateToken(Punctuation.Punctuators[_text.ToString()]);
                    }
                    else
                    {
                        token = CreateToken(
                            Token.Illegal,
                            _text.ToString(),
                            "Unrecognized literal."
                        );
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

                    case '"':
                        return CreateToken(Token.StringLiteral, escaping.ToString());

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

                            case '\n':
                            case '\r':
                            case '\0':
                                return CreateToken(
                                    Token.Illegal,
                                    _text.ToString(),
                                    "Unterminated block comment."
                                );
                        }
                        break;

                    case '\n':
                    case '\r':
                    case '\0':
                        return CreateToken(
                            Token.Illegal,
                            _text.ToString(),
                            "Unterminated block comment."
                        );
                }
            }
        }

        private TokenValue ReadRegExp()
        {
            return CreateToken(Token.Identifier);
        }

        private TokenValue ReadOperator()
        {
            while (true)
            {
                if (IsOperator(Peek()))
                {
                    Advance();
                }
                else
                {
                    return CreateToken(Punctuation.Operators[_text.ToString()]);
                }
            }
        }

        private TokenValue ReadNumber()
        {
            while (true)
            {
                var c = Peek();
                if (char.IsDigit(c) || ".-+abcdefx".Contains(char.ToLower(c)))
                {
                    Advance();
                }
                else
                {
                    return CreateToken(Token.NumberLiteral);
                }
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
                    // look up keyword
                    var text = _text.ToString();
                    if (Punctuation.FutureReservedWord.Contains(text))
                    {
                        return CreateToken(
                            Token.Illegal,
                            text,
                            "This is a future reserved keyword."
                        );
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
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '.';
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
            return "(){}[];,".Contains(c.ToString());
        }

        private char Peek()
        {
            return (char)_textReader.Peek();
        }

        private char Advance()
        {
            var c = char.MinValue;
            if (_textReader.Read(_buffer, 0, 1) > 0)
            {
                c = _buffer[0];
                _text.Append(_buffer);

                if (c == '\n')
                {
                    _line++;
                    _column = 0;
                }
                else
                {
                    _column++;
                }
            }

            return c;
        }

        private TokenValue CreateToken(Token tokenId, string literal = "", string message = "")
        {
            var position = CreatePosition();
            var text = string.IsNullOrEmpty(literal) ? position.Text : literal;

            _startLine = _line;
            _startColumn = _column;
            _text.Clear();

            return new TokenValue
            {
                TokenId = tokenId,
                Literal = text,
                Message = message,
            };
        }

        private Position CreatePosition()
        {
            return new Position(_text.ToString(), _startLine, _line, _startColumn, _column);
        }
    }

    struct Position
    {
        public string Text { get; private set; }
        public int StartLine { get; private set; }
        public int EndLine { get; private set; }
        public int StartColumn { get; private set; }
        public int EndColumn { get; private set; }

        public Position(string text, int startLine, int endLine, int startColumn, int endColumn)
            : this()
        {
            Text = text;
            StartLine = startLine;
            EndLine = endLine;
            StartColumn = startColumn;
            EndColumn = endColumn;
        }
    }

    static class Punctuation
    {
        // Punctuators
        public static IDictionary<string, Token> Punctuators = new Dictionary<string, Token>
        {
            { "(", Token.LeftBracket },
            { ")", Token.RightBracket },
            { "{", Token.LeftBrace },
            { "}", Token.RightBrace },
            { "[", Token.LeftSquareBracket },
            { "]", Token.RightSquareBracket },
            { ";", Token.Semicolon },
            { ",", Token.Comma },
        };

        // Operators
        public static IDictionary<string, Token> Operators = new Dictionary<string, Token>
        {
            { "?", Token.Question },
            { ":", Token.Colon },
            { "~", Token.Tilde },
            { "*", Token.Asterisk },
            { "*=", Token.AsteriskEqual },
            { "%", Token.Percent },
            { "%=", Token.PercentEqual },
            { "^", Token.Carot },
            { "^=", Token.CarotEqual },
            { "/", Token.Slash },
            { "/=", Token.SlashEqual },
            { "&", Token.Ampersand },
            { "&&", Token.Ampersand2 },
            { "&&=", Token.AmpersandEqual },
            { "|", Token.Pipe },
            { "||", Token.Pipe2 },
            { "|=", Token.PipeEqual },
            { "=", Token.Equal },
            { "==", Token.Equal2 },
            { "===", Token.Equal3 },
            { "!", Token.Exclamation },
            { "!=", Token.ExclamationEqual },
            { "!==", Token.ExclamationEqual2 },
            { "+", Token.Plus },
            { "++", Token.Plus2 },
            { "+=", Token.PlusEqual },
            { "-", Token.Minus },
            { "--", Token.Minus2 },
            { "-=", Token.MinusEqual },
            { "<", Token.LessThan },
            { "<<", Token.LessThan2 },
            { "<=", Token.LessThanEqual },
            { "<<=", Token.LessThan2Equal },
            { ">", Token.GreaterThan },
            { ">>", Token.GreaterThan2 },
            { ">=", Token.GreaterThanEqual },
            { ">>=", Token.GreaterThan2Equal },
            { ">>>", Token.GreaterThan3 },
            { ">>>=", Token.GreaterThan3Equal },
        };

        // reserved keywords
        public static IDictionary<string, Token> Keywords = new Dictionary<string, Token>
        {
            { "break", Token.Break },
            { "case", Token.Case },
            { "catch", Token.Catch },
            { "continue", Token.Continue },
            { "debugger", Token.Debugger },
            { "default", Token.Default },
            { "delete", Token.Delete },
            { "do", Token.Do },
            { "else", Token.Else },
            { "finally", Token.Finally },
            { "for", Token.For },
            { "function", Token.Function },
            { "if", Token.If },
            { "in", Token.In },
            { "instanceof", Token.Instanceof },
            { "new", Token.New },
            { "return", Token.Return },
            { "switch", Token.Switch },
            { "this", Token.This },
            { "throw", Token.Throw },
            { "try", Token.Try },
            { "typeof", Token.Typeof },
            { "var", Token.Var },
            { "void", Token.Void },
            { "while", Token.While },
            { "with", Token.With },
            { "true", Token.True },
            { "false", Token.False },
            { "null", Token.Null },
        };

        // future reserved keywords
        public static readonly string[] FutureReservedWord = new[]
        {
            "class",
            "const",
            "enum",
            "export",
            "extends",
            "import",
            "super",
            "implements",
            "interface",
            "let",
            "package",
            "private",
            "protected",
            "public",
            "static",
            "yield",
        };
    }
}
