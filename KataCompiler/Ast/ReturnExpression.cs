#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ReturnExpression : IExpression
{
    public IExpression Expr { get; private set; }

    public ReturnExpression(IExpression expr)
    {
        Expr = expr;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("return: ");
        if (Expr != null)
        {
            Expr.AppendTo(sb);
        }
    }
}
