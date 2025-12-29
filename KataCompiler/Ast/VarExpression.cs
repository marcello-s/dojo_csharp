#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class VarExpression(IExpression exprs) : IExpression
{
    public IExpression Exprs { get; private set; } = exprs;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("var: ");
        Exprs.AppendTo(sb);
        sb.AppendLine();
    }
}
