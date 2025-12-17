#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public abstract class TextSpanFilterBase : ITextSpanFilter
{
    private readonly ITextSpanFilter _filter;

    protected TextSpanFilterBase(ITextSpanFilter filter)
    {
        _filter = filter;
    }

    public IEnumerable<TextSpan> Apply(IEnumerable<TextSpan> spans)
    {
        return DoFilter(_filter != null ? _filter.Apply(spans) : spans);
    }

    protected abstract IEnumerable<TextSpan> DoFilter(IEnumerable<TextSpan> spans);
}
