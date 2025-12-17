#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public class CStyleCommentFilter : TextSpanFilterBase
{
    public const string SinglelineCommentMarker = "//";
    public const string MultilineCommentBeginMarker = "/*";
    public const string MultilineCommentEndMarker = "*/";
    private bool _multilineCommentMode;
    private bool _singlelineCommentMode;

    public CStyleCommentFilter()
        : base(null) { }

    public CStyleCommentFilter(ITextSpanFilter filter)
        : base(filter) { }

    protected override IEnumerable<TextSpan> DoFilter(IEnumerable<TextSpan> spans)
    {
        foreach (var span in spans)
        {
            if (!_multilineCommentMode)
            {
                TextSpan partialSpan;
                if (TryCreatePartialTextSpanFromSinglelineComment(span, out partialSpan))
                {
                    yield return partialSpan;
                    continue;
                }
            }

            IEnumerable<TextSpan> partialSpans;
            if (TryCreatePartialTextSpanFromMultilineComment(span, out partialSpans))
            {
                foreach (var textSpan in partialSpans)
                {
                    yield return textSpan;
                }
                continue;
            }

            if (!(_singlelineCommentMode || _multilineCommentMode))
            {
                yield return span;
            }
        }
    }

    private bool TryCreatePartialTextSpanFromSinglelineComment(
        TextSpan span,
        out TextSpan partialSpan
    )
    {
        var success = false;
        partialSpan = new TextSpan();

        if (!_singlelineCommentMode && CanSetSingleCommentMode(span))
        {
            _singlelineCommentMode = true;

            var commentMarkerIndex = span.Text.IndexOf(
                SinglelineCommentMarker,
                StringComparison.InvariantCulture
            );
            if (commentMarkerIndex > 0)
            {
                partialSpan = CreatePartialTextSpan(span, 0, commentMarkerIndex);
                success = true;
            }
        }

        if (_singlelineCommentMode && CanResetSingleCommentMode(span))
        {
            _singlelineCommentMode = false;
        }

        return success;
    }

    private bool TryCreatePartialTextSpanFromMultilineComment(
        TextSpan span,
        out IEnumerable<TextSpan> partialSpans
    )
    {
        var success = false;
        var partialSpanList = new List<TextSpan>();

        if (!_multilineCommentMode && CanSetMultilineCommentMode(span))
        {
            _multilineCommentMode = true;
            var tmpSpan = span;

            var commentMarkerIndex = span.Text.IndexOf(
                MultilineCommentBeginMarker,
                StringComparison.InvariantCulture
            );
            if (commentMarkerIndex > 0)
            {
                partialSpanList.Add(CreatePartialTextSpan(span, 0, commentMarkerIndex));
                success = true;

                tmpSpan = CreatePartialTextSpan(
                    span,
                    commentMarkerIndex,
                    span.Text.Length - commentMarkerIndex
                );
            }

            // the same span may contain more comment markers - slice and dice until done
            while (CanResetMultilineCommentMode(tmpSpan))
            {
                var beginMarkerIndex = tmpSpan.Text.IndexOf(
                    MultilineCommentBeginMarker,
                    StringComparison.InvariantCulture
                );
                if (beginMarkerIndex > 0)
                {
                    partialSpanList.Add(CreatePartialTextSpan(tmpSpan, 0, beginMarkerIndex));
                }

                var markerIndex = tmpSpan.Text.IndexOf(
                    MultilineCommentEndMarker,
                    StringComparison.InvariantCulture
                );
                if (markerIndex >= tmpSpan.Text.Length - MultilineCommentEndMarker.Length)
                {
                    _multilineCommentMode = false;
                    break;
                }
                markerIndex += MultilineCommentEndMarker.Length;
                tmpSpan = CreatePartialTextSpan(
                    tmpSpan,
                    markerIndex,
                    tmpSpan.Text.Length - markerIndex
                );
            }
        }

        if (_multilineCommentMode && CanResetMultilineCommentMode(span))
        {
            _multilineCommentMode = false;
            success = true; // always true to skip emitting a span like "*/"

            var commentMarkerIndex = span.Text.IndexOf(
                MultilineCommentEndMarker,
                StringComparison.InvariantCulture
            );
            if (span.Text.Length > MultilineCommentEndMarker.Length)
            {
                commentMarkerIndex += MultilineCommentEndMarker.Length;
                partialSpanList.Add(
                    CreatePartialTextSpan(
                        span,
                        commentMarkerIndex,
                        span.Text.Length - commentMarkerIndex
                    )
                );
            }
        }

        partialSpans = partialSpanList;
        return success;
    }

    private static bool CanSetSingleCommentMode(TextSpan span)
    {
        return span.SpanType == SpanTypeEnum.Text
            && span.Text.Contains(SinglelineCommentMarker)
            && !span.Text.Contains(MultilineCommentEndMarker + MultilineCommentBeginMarker);
    }

    private static bool CanResetSingleCommentMode(TextSpan span)
    {
        return span.SpanType == SpanTypeEnum.EndOfLine;
    }

    private static bool CanSetMultilineCommentMode(TextSpan span)
    {
        return span.SpanType == SpanTypeEnum.Text
            && span.Text.Contains(MultilineCommentBeginMarker);
    }

    private static bool CanResetMultilineCommentMode(TextSpan span)
    {
        return span.SpanType == SpanTypeEnum.Text && span.Text.Contains(MultilineCommentEndMarker);
    }

    private static TextSpan CreatePartialTextSpan(
        TextSpan span,
        int subStringStart,
        int subStringLength
    )
    {
        var subString = span.Text.Substring(subStringStart, subStringLength);
        return new TextSpan(SpanTypeEnum.Text, span.Line, span.Begin, span.End, subString);
    }
}
