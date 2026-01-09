#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Parser;

class ErrorReporter : IErrorReporter
{
    private readonly IList<ReportEntry> entries = new List<ReportEntry>();
    private int textLine;

    public int NumberOfErrors
    {
        get { return entries.Count(e => e.Kind == ReportEntryKind.Error); }
    }

    public int NumberOfWarnings
    {
        get { return entries.Count(e => e.Kind == ReportEntryKind.Warning); }
    }

    public void AddError(TokenValue token, string message)
    {
        entries.Add(
            new ReportEntry
            {
                Kind = ReportEntryKind.Error,
                Token = token,
                Message = message,
            }
        );
    }

    public void AddWarning(TokenValue token, string message)
    {
        entries.Add(
            new ReportEntry
            {
                Kind = ReportEntryKind.Warning,
                Token = token,
                Message = message,
            }
        );
    }

    public string Render(TextReader textReader)
    {
        var sb = new StringBuilder();

        sb.AppendFormat("errors: {0} - warnings: {1}", NumberOfErrors, NumberOfWarnings);
        sb.AppendLine();
        foreach (var reportEntry in entries)
        {
            var src = reportEntry.Token.SrcPosition;
            var line = AdvanceReaderToTextLine(src.StartLine, textReader);
            sb.AppendLine(line);
            sb.AppendFormat(
                "({0},{1}) {2}: '{3}'",
                src.StartLine,
                src.StartColumn,
                reportEntry.Kind,
                reportEntry.Message
            );
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string? AdvanceReaderToTextLine(int textLine, TextReader textReader)
    {
        var lines = textLine - this.textLine;
        string? line = string.Empty;

        for (var i = 0; i < lines; ++i)
        {
            line = textReader.ReadLine();
        }

        this.textLine = textLine;
        return line;
    }

    private enum ReportEntryKind
    {
        Warning,
        Error,
    }

    private struct ReportEntry
    {
        public ReportEntryKind Kind { get; set; }
        public TokenValue Token { get; set; }
        public string Message { get; set; }
    }
}
