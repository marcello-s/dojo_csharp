#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using KataCompiler.Ast;

namespace KataCompiler.Compiler;

class CompilerErrorReporter : ICompilerErrorReporter
{
    private readonly IList<ReportEntry> _entries = new List<ReportEntry>();

    public int NumberOfErrors
    {
        get { return _entries.Count(e => e.Kind == ReportEntryKind.Error); }
    }
    public int NumberOfWarnings
    {
        get { return _entries.Count(e => e.Kind == ReportEntryKind.Warning); }
    }

    public void AddError(IExpression expr, string message)
    {
        _entries.Add(
            new ReportEntry
            {
                Kind = ReportEntryKind.Error,
                Expr = expr,
                Message = message,
            }
        );
    }

    public void AddWarning(IExpression expr, string message)
    {
        _entries.Add(
            new ReportEntry
            {
                Kind = ReportEntryKind.Warning,
                Expr = expr,
                Message = message,
            }
        );
    }

    public string Render()
    {
        var sb = new StringBuilder();

        sb.AppendFormat("errors: {0} - warnings: {1}", NumberOfErrors, NumberOfWarnings);
        sb.AppendLine();

        foreach (var entry in _entries)
        {
            entry.Expr.AppendTo(sb);
            sb.AppendFormat(" - '{0}'", entry.Message);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private enum ReportEntryKind
    {
        Warning,
        Error,
    }

    private struct ReportEntry
    {
        public ReportEntryKind Kind { get; set; }
        public IExpression Expr { get; set; }
        public string Message { get; set; }
    }
}
