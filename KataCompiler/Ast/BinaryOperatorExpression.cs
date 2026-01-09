#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class BinaryOperatorExpression(IExpression left, Parser.Token tokenId, IExpression right)
    : IExpression
{
    public IExpression Left { get; private set; } = left;
    public Parser.Token TokenId { get; private set; } = tokenId;
    public IExpression Right { get; private set; } = right;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("(");
        Left.AppendTo(sb);
        sb.Append(" ");
        sb.Append(TokenId);
        sb.Append(" ");
        Right.AppendTo(sb);
        sb.Append(")");
    }
}
