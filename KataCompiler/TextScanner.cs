#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler;

public class TextScanner
{
    public IEnumerable<TextSpan> Scan(TextReader textReader)
    {
        var buffer = new char[1];
        var newLineBuffer = new char[2];
        int line = 0,
            column = 0;
        int spanColumn = 0;
        var sb = new StringBuilder();

        var wordMode = !Char.IsWhiteSpace((char)textReader.Peek());
        var stringMode = IsQuote((char)textReader.Peek());

        while (textReader.Read(buffer, 0, 1) > 0)
        {
            var c = buffer[0];
            newLineBuffer[1] = c;
            sb.Append(c);

            if (CanResetStringmode(stringMode, c, GetPreviousChar(sb)))
            {
                stringMode = false;
            }
            else if (CanSetStringmode(stringMode, c, GetPreviousChar(sb)))
            {
                stringMode = true;
            }

            if (CanResetWordmode(wordMode, stringMode, c))
            {
                // reset word mode
                wordMode = false;
                // emit text token if column not at line beginning
                // record current column in spanColumn
                if (column > 0)
                {
                    var begin = new TextLocation(spanColumn);
                    spanColumn = column - 1;
                    var end = new TextLocation(spanColumn);
                    sb = sb.Remove(sb.Length - 1, 1);
                    var text = sb.ToString();
                    sb.Clear();
                    yield return new TextSpan(SpanTypeEnum.Text, line, begin, end, text);
                }
            }
            else if (CanSetWordmode(wordMode, c))
            {
                // set word mode
                wordMode = true;
                // emit whitespace token if column not at beginning
                // record current column in spanColumn
                if (column > 0)
                {
                    var begin = new TextLocation(spanColumn);
                    spanColumn = column - 1;
                    var end = new TextLocation(spanColumn);
                    sb = sb.Remove(0, sb.Length - 1);
                    yield return new TextSpan(SpanTypeEnum.Whitespace, line, begin, end);
                }
            }

            if (IsBufferEqualToNewLine(newLineBuffer))
            {
                if (column > 2)
                {
                    // emit text or whitespace token if there is a span from column beginning
                    var spanBegin = new TextLocation(spanColumn);
                    var spanEnd = new TextLocation(column - 1);
                    var spanType = wordMode ? SpanTypeEnum.Text : SpanTypeEnum.Whitespace;
                    if (spanType == SpanTypeEnum.Text)
                    {
                        sb = sb.Remove(sb.Length - 2, 2);
                        var text = sb.ToString();
                        sb.Clear();
                        yield return new TextSpan(spanType, line, spanBegin, spanEnd, text);
                    }
                    else
                    {
                        yield return new TextSpan(spanType, line, spanBegin, spanEnd);
                    }
                }
                // emit endofline token
                // reset all column/line counters
                var begin = new TextLocation(column - 1);
                var end = new TextLocation(column);
                var currentLine = line;
                spanColumn = column = 0;
                line++;
                sb.Clear();
                yield return new TextSpan(SpanTypeEnum.EndOfLine, currentLine, begin, end);
            }
            else
            {
                column++;
                newLineBuffer[0] = newLineBuffer[1];
            }
        }

        // emit remaining text
        var spBegin = new TextLocation(spanColumn);
        var spEnd = new TextLocation(column);
        var spText = sb.ToString();
        if ((spEnd.Column - spBegin.Column) > 0 && spText.Length > 0)
        {
            yield return new TextSpan(SpanTypeEnum.Text, line, spBegin, spEnd, spText);
        }
    }

    private static bool CanResetWordmode(bool wordMode, bool stringMode, char c)
    {
        return wordMode && !stringMode && IsWhiteSpaceButNotEndOfLine(c);
    }

    private static bool IsWhiteSpaceButNotEndOfLine(char c)
    {
        return Char.IsWhiteSpace(c) && !(c.Equals('\r') || c.Equals('\n'));
    }

    private static bool CanSetWordmode(bool wordMode, char c)
    {
        return !wordMode && !Char.IsWhiteSpace(c);
    }

    private static bool IsBufferEqualToNewLine(char[] buffer)
    {
        return new String(buffer).Equals(Environment.NewLine);
    }

    private static bool IsQuote(char c)
    {
        return c.Equals('"');
    }

    private static bool IsEscapedQuote(char c, char previous)
    {
        return c.Equals('"') && previous.Equals('\\');
    }

    private static char GetPreviousChar(StringBuilder sb)
    {
        var c = char.MinValue;

        if (sb.Length > 1)
        {
            c = sb[sb.Length - 2];
        }

        return c;
    }

    private static bool CanSetStringmode(bool stringMode, char c, char previous)
    {
        return !stringMode && IsQuote(c) && !IsEscapedQuote(c, previous);
    }

    private static bool CanResetStringmode(bool stringMode, char c, char previous)
    {
        return stringMode && IsQuote(c) && !IsEscapedQuote(c, previous);
    }
}
