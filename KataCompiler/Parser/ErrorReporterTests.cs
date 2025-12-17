#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace KataCompiler.Parser;

[TestFixture]
public class ErrorReporterTests
{
    [Test]
    public void CreateReport_jQuery_extract()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var errorReporter = new ErrorReporter();

        var tokenValue1 = new TokenValue
        {
            SrcPosition = new Position("typeof", 22, 22, 7, 13),
            Message = "Pretend unknown token",
        };
        errorReporter.AddError(tokenValue1, "Unknown keyword \"typeof\"");

        var tokenValue2 = new TokenValue { SrcPosition = new Position("src", 57, 57, 15, 18) };
        errorReporter.AddError(tokenValue2, "\"src\" not defined");

        var report = CreateReport(errorReporter, input);
        Console.WriteLine(report);
    }

    private static string CreateReport(IErrorReporter errorReporter, string input)
    {
        var bytes = Encoding.Default.GetBytes(input);
        string report;

        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            report = errorReporter.Render(sr);
        }

        return report;
    }
}
