#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ConditionalExpression : IExpression
{
    public IExpression Condition { get; private set; }
    public IExpression TruthyBranch { get; private set; }
    public IExpression FalsyBranch { get; private set; }

    public ConditionalExpression(
        IExpression condition,
        IExpression truthyBranch,
        IExpression falsyBranch
    )
    {
        Condition = condition;
        TruthyBranch = truthyBranch;
        FalsyBranch = falsyBranch;
    }

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
