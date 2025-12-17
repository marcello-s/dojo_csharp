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
public class TextScannerTests
{
    private TextScanner textScanner = null!;

    [SetUp]
    public void Setup()
    {
        textScanner = new TextScanner();
    }

    [Test]
    public void TextScanner_Scan_TextSpan()
    {
        Console.Out.WriteLine("tab is whitespace: {0}", Char.IsWhiteSpace('\t'));
        Console.Out.WriteLine("tab is control: {0}", Char.IsControl('\t'));
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.TextScannerInput.txt");
        var input = new StreamReader(stream!).ReadToEnd();

        var textScannerInput = Encoding.Default.GetBytes(input);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var textSpans = textScanner.Scan(textReader);
            Assert.That(textSpans, Is.Not.Null);
            foreach (var textSpan in textSpans)
            {
                Console.Out.WriteLine(
                    "type: '{0}', line: '{1}''{2}-{3}'",
                    textSpan.SpanType,
                    textSpan.Line,
                    textSpan.Begin.Column,
                    textSpan.End.Column
                );
            }
        }
    }

    [Test]
    public void TextScanner_Scan_TextSpanContainsText()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.TextScannerInput.txt");
        var input = new StreamReader(stream!).ReadToEnd();

        var textScannerInput = Encoding.Default.GetBytes(input);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var textSpans = textScanner
                .Scan(textReader)
                .Where(span => span.SpanType == SpanTypeEnum.Text);
            Assert.That(textSpans, Is.Not.Null);
            foreach (var textSpan in textSpans)
            {
                Console.Out.WriteLine("text: '{0}'", textSpan.Text);
                if (textSpan.Text.Contains('\t'))
                {
                    Console.Out.WriteLine(
                        "oops tab - line: '{0}''{1}-{2}'",
                        textSpan.Line,
                        textSpan.Begin.Column,
                        textSpan.End.Column
                    );
                }
            }
        }
    }

    [Test]
    public void TextScanner_ScanScript_TextSpanContainsText()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var textScannerInput = Encoding.Default.GetBytes(input);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var textSpans = textScanner
                .Scan(textReader)
                .Where(span => span.SpanType == SpanTypeEnum.Text);
            Assert.That(textSpans, Is.Not.Null);
            foreach (var textSpan in textSpans)
            {
                Console.Out.WriteLine("text: '{0}'", textSpan.Text);
            }
        }
    }

    [Test]
    public void TextScanner_Scan_StringLiteralsArePreserved()
    {
        const string text = "The \"quick brown fox\" jumps over the \"\\\"lazy\\\"\" dog.";
        var textScannerInput = Encoding.Default.GetBytes(text);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var textSpans = textScanner
                .Scan(textReader)
                .Where(span => span.SpanType == SpanTypeEnum.Text);
            Assert.That(textSpans, Is.Not.Null);
            foreach (var textSpan in textSpans)
            {
                Console.Out.WriteLine("text: '{0}'", textSpan.Text);
            }
        }
    }
}
