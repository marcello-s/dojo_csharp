#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class SwitchExpression(IExpression switchExpr, IExpression caseExpr) : IExpression
{
    public IExpression SwitchExpr { get; private set; } = switchExpr;
    public IExpression CaseExpr { get; private set; } = caseExpr;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("switch: ");
        SwitchExpr.AppendTo(sb);
        sb.AppendLine("{");
        CaseExpr.AppendTo(sb);
        sb.AppendLine("}");
    }
}
