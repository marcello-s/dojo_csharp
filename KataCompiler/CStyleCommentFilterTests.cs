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
public class CStyleCommentFilterTests
{
    private TextScanner textScanner = null!;

    [SetUp]
    public void Setup()
    {
        textScanner = new TextScanner();
    }

    [Test]
    public void AllResources()
    {
        var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        foreach (var name in names)
        {
            Console.WriteLine(name);
        }
    }

    [Test]
    public void TextScanner_ScanScript_CStyleCommentIsFiltered()
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
            var cStyleCommentFilter = new CStyleCommentFilter();
            var textSpans = cStyleCommentFilter.Apply(spans);
            Assert.That(textSpans, Is.Not.Null);
            Assert.That(
                textSpans.Any(span =>
                    span.Text.Contains(CStyleCommentFilter.SinglelineCommentMarker)
                    || span.Text.Contains(CStyleCommentFilter.MultilineCommentBeginMarker)
                    || span.Text.Contains(CStyleCommentFilter.MultilineCommentEndMarker)
                ),
                Is.False
            );
            //foreach (var textSpan in textSpans)
            //{
            //    Console.WriteLine("- " + textSpan.Text);
            //}
        }
    }

    [Test]
    public void TextScanner_ScanText_SinglelineCommentIsFiltered()
    {
        var src = "var a;///*comment*/" + Environment.NewLine + "return a;";
        var textScannerInput = Encoding.Default.GetBytes(src);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var cStyleCommentFilter = new CStyleCommentFilter();
            var textSpans = cStyleCommentFilter.Apply(spans);
            Assert.That(textSpans, Is.Not.Null);
            Assert.That(textSpans.Count(), Is.EqualTo(7));
            //foreach (var textSpan in textSpans)
            //{
            //    Console.WriteLine("- " + textSpan.Text);
            //}
        }
    }

    [Test]
    public void TextScanner_ScanText_MultilineCommentIsFiltered()
    {
        const string src = "var/**//**/a;/**/";
        var textScannerInput = Encoding.Default.GetBytes(src);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var cStyleCommentFilter = new CStyleCommentFilter();
            var textSpans = cStyleCommentFilter.Apply(spans);
            Assert.That(textSpans, Is.Not.Null);
            Assert.That(textSpans.Count(), Is.EqualTo(2));
            //foreach (var textSpan in textSpans)
            //{
            //    Console.WriteLine("- " + textSpan.Text);
            //}
        }
    }
}
