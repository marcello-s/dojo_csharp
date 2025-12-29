#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ForExpression(
    IEnumerable<IExpression> initExprs,
    IExpression? conditionExpr,
    IExpression? incrementExpr,
    IExpression expr
) : IExpression
{
    public IEnumerable<IExpression> InitExprs { get; private set; } = initExprs;
    public IExpression? ConditionExpr { get; private set; } = conditionExpr;
    public IExpression? IncrementExpr { get; private set; } = incrementExpr;
    public IExpression Expr { get; private set; } = expr;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("for: ");
        foreach (var initExpr in InitExprs)
        {
            sb.Append("init=");
            initExpr.AppendTo(sb);
        }

        if (ConditionExpr != null)
        {
            sb.Append(" cond=");
            ConditionExpr.AppendTo(sb);
        }

        if (IncrementExpr != null)
        {
            sb.Append(" inc=");
            IncrementExpr.AppendTo(sb);
        }

        sb.AppendLine("{");
        Expr.AppendTo(sb);
        sb.AppendLine("}");
    }
}
