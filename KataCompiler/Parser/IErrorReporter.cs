#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Parser;

interface IErrorReporter
{
    int NumberOfErrors { get; }
    int NumberOfWarnings { get; }
    void AddError(TokenValue token, string message);
    void AddWarning(TokenValue token, string message);
    string Render(TextReader textReader);
}
