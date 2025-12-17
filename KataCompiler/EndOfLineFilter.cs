#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public class EndOfLineFilter : TextSpanFilterBase
{
    public EndOfLineFilter()
        : base(null) { }

    public EndOfLineFilter(ITextSpanFilter filter)
        : base(filter) { }

    protected override IEnumerable<TextSpan> DoFilter(IEnumerable<TextSpan> spans)
    {
        return spans.Where(span => span.SpanType != SpanTypeEnum.EndOfLine);
    }
}
