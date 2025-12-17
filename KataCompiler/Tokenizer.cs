#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler;

public class Tokenizer
{
    // punctuation character dictionary - keep in sync with Token enum
    private int[] _punctuationListOffsetTable;
    public readonly IDictionary<char, string[]> Punctuation = new Dictionary<char, string[]>
    {
        { '(', new[] { "(" } },
        { ')', new[] { ")" } },
        { '{', new[] { "{" } },
        { '}', new[] { "}" } },
        { '[', new[] { "[" } },
        { ']', new[] { "]" } },
        { '?', new[] { "?" } },
        { ':', new[] { ":" } },
        { ';', new[] { ";" } },
        { ',', new[] { "," } },
        { '.', new[] { "." } },
        { '~', new[] { "~" } },
        { '*', new[] { "*", "*=" } },
        { '%', new[] { "%", "%=" } },
        { '^', new[] { "^", "^=" } },
        { '/', new[] { "/", "/=" } },
        { '&', new[] { "&", "&&", "&=" } },
        { '|', new[] { "|", "||", "|=" } },
        { '=', new[] { "=", "==", "===" } },
        { '!', new[] { "!", "!=", "!==" } },
        { '+', new[] { "+", "++", "+=" } },
        { '-', new[] { "-", "--", "-=" } },
        { '<', new[] { "<", "<<", "<=", "<<=" } },
        { '>', new[] { ">", ">>", ">=", ">>=", ">>>", ">>>=" } },
    };

    // reserved keywords
    public readonly string[] ReservedKeyword = new[]
    {
        "break",
        "case",
        "catch",
        "continue",
        "debugger",
        "default",
        "delete",
        "do",
        "else",
        "finally",
        "for",
        "function",
        "if",
        "in",
        "instanceof",
        "new",
        "return",
        "switch",
        "this",
        "throw",
        "try",
        "typeof",
        "var",
        "void",
        "while",
        "with",
        "true",
        "false",
        "null",
    };

    // future reserved keywords
    public readonly string[] FutureReservedWord = new[]
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

    public readonly char[] NumberPunctuation = new[] { '+', '-' };
    public const char NumberDecimalPoint = '.';
    public const string HexNumberPrefix = "0x";
    public const char StringMark = '"';
    public const string EscapedStringMark = "\\\"";

    public IEnumerable<TokenValue> Tokenize(IEnumerable<TextSpan> spans)
    {
        var buffer = new StringBuilder();
        _punctuationListOffsetTable = ComputePunctuationListOffsetTable();

        // these fields keep the state while tokenizing
        // an fsm class should be used instead, with the
        // tokenizer as a stateful object
        string[] punctuation = null;
        var punctuationIndex = 0;
        var numberLiteralMode = false;
        var stringLiteralMode = false;

        foreach (var span in spans)
        {
            foreach (var c in span.Text)
            {
                var accuBuffer = buffer.ToString();

                // clear buffer for punctuation
                if (punctuation != null && !Punctuation.ContainsKey(c) && buffer.Length > 0)
                {
                    yield return CreatePunctuationTokenValue(
                        punctuation,
                        punctuationIndex,
                        span,
                        accuBuffer
                    );
                    punctuation = null;
                    buffer.Clear();
                }

                // tokenizing number literal - number mode on
                if (!numberLiteralMode && char.IsDigit(c))
                {
                    numberLiteralMode = true;
                    buffer.Clear();
                }

                // tokenizing number literal - number mode off
                if (
                    numberLiteralMode
                        && Punctuation.ContainsKey(c)
                        && !(
                            c.Equals(NumberDecimalPoint)
                            || c.Equals(NumberPunctuation[0])
                            || c.Equals(NumberPunctuation[1])
                        )
                    || (
                        numberLiteralMode
                        && IsNumberLiteralTerminated(c, accuBuffer, NumberPunctuation)
                    )
                    || (
                        numberLiteralMode
                        && IsHexNumberLiteralTerminated(c, accuBuffer, NumberPunctuation)
                    )
                )
                {
                    numberLiteralMode = false;
                    yield return new TokenValue
                    {
                        Span = span,
                        TokenId = Token.NumberLiteral,
                        Literal = accuBuffer,
                    };
                }

                // check for illegal non-digit number characters
                if (
                    numberLiteralMode
                    && !(char.IsDigit(c) || ".-+abcdefx".Contains(char.ToLower(c)))
                )
                {
                    var msg = string.Format("'{0}' illegal character in number literal.", c);
                    yield return new TokenValue
                    {
                        Span = span,
                        TokenId = Token.Illegal,
                        Message = msg,
                    };
                }

                // tokenizing string literal - mode on
                if (!stringLiteralMode && c.Equals(StringMark))
                {
                    stringLiteralMode = true;
                    buffer.Clear();
                }

                // tokenizing string literal - mode off
                if (
                    stringLiteralMode
                    && accuBuffer.Length > 1
                    && accuBuffer.EndsWith(StringMark.ToString())
                    && !accuBuffer.EndsWith(EscapedStringMark)
                )
                {
                    stringLiteralMode = false;
                    yield return new TokenValue
                    {
                        Span = span,
                        TokenId = Token.StringLiteral,
                        Literal = accuBuffer,
                    };
                }

                // tokenizing punctuation
                if (
                    !(numberLiteralMode || stringLiteralMode)
                    && punctuation == null
                    && Punctuation.ContainsKey(c)
                )
                {
                    punctuation = Punctuation[c];
                    punctuationIndex = Punctuation.Keys.ToList().IndexOf(c);

                    // emit reserved keyword
                    if (ReservedKeyword.Contains(accuBuffer))
                    {
                        yield return CreatedReservedKeywordTokenValue(accuBuffer, span);
                    }

                    // tokenizing future reserved keyword -> error
                    if (FutureReservedWord.Contains(accuBuffer))
                    {
                        var msg = string.Format("'{0}' is a reserved future keyword.", accuBuffer);
                        yield return new TokenValue
                        {
                            Span = span,
                            TokenId = Token.Illegal,
                            Message = msg,
                        };
                    }

                    // emit single punctuation
                    if (punctuation.Length == 1)
                    {
                        yield return CreatePunctuationTokenValue(
                            punctuation,
                            punctuationIndex,
                            span,
                            c.ToString()
                        );
                        punctuation = null;
                    }

                    Console.WriteLine("-> (char loop) buffer: '{0}'", buffer);
                    buffer.Clear();
                }

                buffer.Append(c);
            }

            // tokenizing string literal - mode off
            var accu = buffer.ToString();
            if (
                stringLiteralMode
                && accu.Length > 1
                && accu.EndsWith(StringMark.ToString())
                && !accu.EndsWith(EscapedStringMark)
            )
            {
                stringLiteralMode = false;
                yield return new TokenValue
                {
                    Span = span,
                    TokenId = Token.StringLiteral,
                    Literal = accu,
                };
            }

            // tokenizing punctuation
            if (punctuation != null && punctuation.Contains(accu))
            {
                yield return CreatePunctuationTokenValue(punctuation, punctuationIndex, span, accu);
                punctuation = null;
            }

            // tokenizing reserved keyword
            if (ReservedKeyword.Contains(accu))
            {
                yield return CreatedReservedKeywordTokenValue(accu, span);
            }

            // tokenizing future reserved keyword -> error
            if (FutureReservedWord.Contains(accu))
            {
                yield return new TokenValue { Span = span, TokenId = Token.Illegal };
            }

            // -- if no match found at all, there was probably an illegal character

            Console.WriteLine("-> (span loop) buffer: '{0}'", buffer);
            buffer.Clear();
        }
    }

    private int[] ComputePunctuationListOffsetTable()
    {
        var table = new int[Punctuation.Count];
        var offset = 0;

        for (var i = 0; i < Punctuation.Count; ++i)
        {
            table[i] = offset;
            offset += Punctuation.ElementAt(i).Value.Count();
        }

        return table;
    }

    private TokenValue CreatePunctuationTokenValue(
        IEnumerable<string> punctuation,
        int punctuationIndex,
        TextSpan span,
        string accu
    )
    {
        var listIndex = punctuation.ToList().IndexOf(accu);
        var tokenIndex = _punctuationListOffsetTable[punctuationIndex] + listIndex;
        var tokenId = (Token)Enum.ToObject(typeof(Token), tokenIndex);
        return new TokenValue { Span = span, TokenId = tokenId };
    }

    private static TokenValue CreatedReservedKeywordTokenValue(string keyword, TextSpan span)
    {
        var capitalizedWord = Capitalize(keyword);
        var tokenId = (Token)Enum.Parse(typeof(Token), capitalizedWord);
        return new TokenValue { Span = span, TokenId = tokenId };
    }

    private static string Capitalize(string name)
    {
        return name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
    }

    private static bool StringContainsAnyOf(string theString, IEnumerable<char> characters)
    {
        return theString.Any(characters.Contains);
    }

    private static bool IsNumberLiteralTerminated(
        char c,
        string accuBuffer,
        IList<char> numberPunctuation
    )
    {
        return (c.Equals(NumberDecimalPoint) && StringContainsAnyOf(accuBuffer, numberPunctuation))
            || (
                c.Equals(numberPunctuation[0]) && StringContainsAnyOf(accuBuffer, numberPunctuation)
            )
            || (
                c.Equals(numberPunctuation[1]) && StringContainsAnyOf(accuBuffer, numberPunctuation)
            );
    }

    private static bool IsHexNumberLiteralTerminated(
        char c,
        string accuBuffer,
        IList<char> numberPunctuation
    )
    {
        return (
                c.Equals(NumberDecimalPoint)
                || c.Equals(numberPunctuation[0])
                || c.Equals(numberPunctuation[1])
            ) && accuBuffer.ToLower().StartsWith(HexNumberPrefix);
    }
}
