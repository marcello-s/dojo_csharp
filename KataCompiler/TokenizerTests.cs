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
public class TokenizerTests
{
    private TextScanner textScanner = null!;
    private ITextSpanFilter textSpanFilter = null!;

    [SetUp]
    public void Setup()
    {
        textScanner = new TextScanner();
        var whiteSpaceFilter = new WhitespaceFilter();
        var cStyleCommentFilter = new CStyleCommentFilter(whiteSpaceFilter);
        textSpanFilter = new EndOfLineFilter(cStyleCommentFilter);
    }

    [Test]
    public void TextScanner_ScanScript_TokenizerReturnsTokens()
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
            var textSpans = textSpanFilter.Apply(spans);
            var tokenizer = new Tokenizer();
            var tokenValues = tokenizer.Tokenize(textSpans);
            Assert.That(tokenValues, Is.Not.Null);
            foreach (var tokenValue in tokenValues)
            {
                Console.WriteLine(FormatTokenValue(tokenValue));
            }
        }
    }

    [Test]
    public void TextScanner_ScanNumber_TokenizerReturnsNumber()
    {
        var src = "var a=1.5e-3+2; " + Environment.NewLine + "return a;";
        var textScannerInput = Encoding.Default.GetBytes(src);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var textSpans = textSpanFilter.Apply(spans);
            var tokenizer = new Tokenizer();
            var tokenValues = tokenizer.Tokenize(textSpans);
            Assert.That(tokenValues, Is.Not.Null);
            foreach (var tokenValue in tokenValues)
            {
                Console.WriteLine(FormatTokenValue(tokenValue));
            }
        }
    }

    [Test]
    public void TextScanner_ScanNumberHex_TokenizerReturnsNumber()
    {
        const string src = "var a=0xff+2;";
        var textScannerInput = Encoding.Default.GetBytes(src);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var textSpans = textSpanFilter.Apply(spans);
            var tokenizer = new Tokenizer();
            var tokenValues = tokenizer.Tokenize(textSpans);
            Assert.That(tokenValues, Is.Not.Null);
            foreach (var tokenValue in tokenValues)
            {
                Console.WriteLine(FormatTokenValue(tokenValue));
            }
        }
    }

    [Test]
    public void TextScanner_ScanStringLiteral_TokenizerReturnsLiteral()
    {
        const string src = "var txt=\"my string with \\\"quotes.\\\"\";";
        var textScannerInput = Encoding.Default.GetBytes(src);
        using (var fs = new MemoryStream(textScannerInput))
        {
            var textReader = new StreamReader(fs);
            var spans = textScanner.Scan(textReader);
            var textSpans = textSpanFilter.Apply(spans);
            var tokenizer = new Tokenizer();
            var tokenValues = tokenizer.Tokenize(textSpans);
            Assert.That(tokenValues, Is.Not.Null);
            foreach (var tokenValue in tokenValues)
            {
                Console.WriteLine(FormatTokenValue(tokenValue));
            }
        }
    }

    private static string FormatTokenValue(TokenValue tokenValue)
    {
        var sb = new StringBuilder();

        sb.AppendFormat("tok:'{0}' ", tokenValue.TokenId);
        sb.AppendFormat("src:'{0}' ", tokenValue.Span.Text);

        if (!string.IsNullOrEmpty(tokenValue.Literal))
        {
            sb.AppendFormat("lit:'{0}' ", tokenValue.Literal);
        }

        if (!string.IsNullOrEmpty(tokenValue.Message))
        {
            sb.AppendFormat("msg:'{0}'", tokenValue.Message);
        }

        return sb.ToString();
    }
}
