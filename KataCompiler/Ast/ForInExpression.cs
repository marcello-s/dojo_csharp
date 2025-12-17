#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ForInExpression : IExpression
{
    public IExpression ObjectExpr { get; private set; }
    public IExpression Expr { get; private set; }

    public ForInExpression(IExpression objectExpr, IExpression expr)
    {
        ObjectExpr = objectExpr;
        Expr = expr;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("for-in: ");
        sb.Append("object=");
        ObjectExpr.AppendTo(sb);
        sb.AppendLine("{");
        Expr.AppendTo(sb);
        sb.AppendLine("}");
    }
}
