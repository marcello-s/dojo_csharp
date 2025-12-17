#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class AssignExpression : IExpression
{
    public IExpression Left { get; private set; }
    public IExpression Right { get; private set; }
    public object Tag { get; set; }

    public AssignExpression(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("(");
        Left.AppendTo(sb);
        sb.Append(" = ");
        Right.AppendTo(sb);
        sb.Append(")");
    }
}
