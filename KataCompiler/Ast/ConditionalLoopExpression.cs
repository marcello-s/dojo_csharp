#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ConditionalLoopExpression(
    IExpression conditionalExpr,
    IExpression expr,
    bool postEvaluation = false
) : IExpression
{
    public IExpression ConditionalExpr { get; private set; } = conditionalExpr;
    public IExpression Expr { get; private set; } = expr;
    public bool PostEvaluation { get; private set; } = postEvaluation;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        if (PostEvaluation)
        {
            sb.Append("do-");
        }

        sb.Append("while: ");
        ConditionalExpr.AppendTo(sb);
        sb.AppendLine("{");
        Expr.AppendTo(sb);
        sb.AppendLine("}");
    }
}
