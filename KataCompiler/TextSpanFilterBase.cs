#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public abstract class TextSpanFilterBase(ITextSpanFilter? filter) : ITextSpanFilter
{
    public IEnumerable<TextSpan> Apply(IEnumerable<TextSpan> spans)
    {
        return DoFilter(filter != null ? filter.Apply(spans) : spans);
    }

    protected abstract IEnumerable<TextSpan> DoFilter(IEnumerable<TextSpan> spans);
}
