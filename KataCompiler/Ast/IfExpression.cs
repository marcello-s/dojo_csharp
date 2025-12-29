#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class IfExpression(IExpression conditionalExpr, IExpression expr, IExpression? elseExpr)
    : IExpression
{
    public IExpression ConditionalExpr { get; private set; } = conditionalExpr;
    public IExpression Expr { get; private set; } = expr;
    public IExpression? ElseExpr { get; private set; } = elseExpr;

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
