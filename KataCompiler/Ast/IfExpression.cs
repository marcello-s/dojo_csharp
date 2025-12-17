#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class IfExpression : IExpression
{
    public IExpression ConditionalExpr { get; private set; }
    public IExpression Expr { get; private set; }
    public IExpression ElseExpr { get; private set; }

    public IfExpression(IExpression conditionalExpr, IExpression expr, IExpression elseExpr)
    {
        ConditionalExpr = conditionalExpr;
        Expr = expr;
        ElseExpr = elseExpr;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("if: ");
        ConditionalExpr.AppendTo(sb);
        sb.AppendLine("{");
        Expr.AppendTo(sb);
        sb.AppendLine("}");

        if (ElseExpr != null)
        {
            sb.AppendLine("else: {");
            ElseExpr.AppendTo(sb);
            sb.AppendLine("}");
        }
    }
}
