#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Compiler;

interface ICompilerErrorReporter
{
    int NumberOfErrors { get; }
    int NumberOfWarnings { get; }
    void AddError(IExpression expr, string message);
    void AddWarning(IExpression expr, string message);
    string Render();
}
