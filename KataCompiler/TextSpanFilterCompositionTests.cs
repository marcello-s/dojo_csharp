#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace KataCompiler;

[TestFixture]
public class TextSpanFilterCompositionTests
{
    private TextScanner textScanner = null!;

    [SetUp]
    public void Setup()
    {
        textScanner = new TextScanner();
    }

    [Test]
    public void TextScanner_ScanScript_AllCommentIsFiltered()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var textScannerInput = Encoding.Default.GetBytes(input);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var whiteSpaceFilter = new WhitespaceFilter();
            var cStyleCommentFilter = new CStyleCommentFilter(whiteSpaceFilter);
            var endOfLineFilter = new EndOfLineFilter(cStyleCommentFilter);
            var textSpans = endOfLineFilter.Apply(spans);
            Assert.That(textSpans, Is.Not.Null);
            Assert.That(
                textSpans.Any(span =>
                    span.Text.Contains(CStyleCommentFilter.SinglelineCommentMarker)
                    || span.Text.Contains(CStyleCommentFilter.MultilineCommentBeginMarker)
                    || span.Text.Contains(CStyleCommentFilter.MultilineCommentEndMarker)
                    || span.SpanType == SpanTypeEnum.Whitespace
                    || span.SpanType == SpanTypeEnum.EndOfLine
                ),
                Is.False
            );
        }
    }
}
