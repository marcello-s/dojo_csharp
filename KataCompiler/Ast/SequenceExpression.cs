#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class SequenceExpression : IExpression
{
    public IEnumerable<IExpression> Exprs { get; private set; }

    public SequenceExpression(IEnumerable<IExpression> exprs)
    {
        Exprs = exprs;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        foreach (var expr in Exprs)
        {
            if (expr != Exprs.ElementAt(0))
            {
                sb.Append(", ");
            }

            expr.AppendTo(sb);
        }
    }
}
