#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ConditionalExpression(
    IExpression condition,
    IExpression truthyBranch,
    IExpression falsyBranch
) : IExpression
{
    public IExpression Condition { get; private set; } = condition;
    public IExpression TruthyBranch { get; private set; } = truthyBranch;
    public IExpression FalsyBranch { get; private set; } = falsyBranch;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("(");
        Condition.AppendTo(sb);
        sb.Append(" ? ");
        TruthyBranch.AppendTo(sb);
        sb.Append(" : ");
        FalsyBranch.AppendTo(sb);
        sb.Append(")");
    }
}
