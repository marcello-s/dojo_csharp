#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class IllegalExpression : IExpression
{
    public IExpression Left { get; private set; }
    public IExpression Expr { get; private set; }
    public string Message { get; private set; }
    public Parser.TokenValue Token { get; private set; }

    public IllegalExpression(
        IExpression left,
        IExpression expr,
        string message,
        Parser.TokenValue token
    )
    {
        Left = left;
        Expr = expr;
        Message = message;
        Token = token;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("illegal: ");
        if (Left != null)
        {
            Left.AppendTo(sb);
        }

        if (Expr != null)
        {
            Expr.AppendTo(sb);
        }

        sb.AppendLine(Message);
    }
}
