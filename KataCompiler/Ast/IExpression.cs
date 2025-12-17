#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

interface IExpression
{
    R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope);
    void AppendTo(StringBuilder sb);
}
