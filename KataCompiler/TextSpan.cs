#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public struct TextSpan
{
    private readonly SpanTypeEnum _spanType;
    private readonly int _line;
    private readonly TextLocation _begin;
    private readonly TextLocation _end;
    private readonly string _text;

    public SpanTypeEnum SpanType
    {
        get { return _spanType; }
    }
    public int Line
    {
        get { return _line; }
    }
    public TextLocation Begin
    {
        get { return _begin; }
    }
    public TextLocation End
    {
        get { return _end; }
    }
    public string Text
    {
        get { return _text; }
    }

    public TextSpan(SpanTypeEnum spanType, int line, TextLocation begin, TextLocation end)
    {
        if (!Enum.IsDefined(typeof(SpanTypeEnum), spanType))
        {
            throw new ArgumentOutOfRangeException("spanType", "the value is not defined");
        }

        if (line < 0)
        {
            throw new ArgumentException("must be >= 0", "line");
        }

        if (end.Column < begin.Column)
        {
            throw new ArgumentException("must be greater or equal than 'begin'", "end");
        }

        _spanType = spanType;
        _line = line;
        _begin = begin;
        _end = end;
        _text = string.Empty;
    }

    public TextSpan(
        SpanTypeEnum spanType,
        int line,
        TextLocation begin,
        TextLocation end,
        string text
    )
        : this(spanType, line, begin, end)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException("text");
        }

        _text = text;
    }
}
