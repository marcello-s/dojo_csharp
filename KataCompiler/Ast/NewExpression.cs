#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class NewExpression : IExpression
{
    public IExpression CallExpr { get; private set; }

    public NewExpression(IExpression callExpr)
    {
        CallExpr = callExpr;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("new: ");
        CallExpr.AppendTo(sb);
    }
}
