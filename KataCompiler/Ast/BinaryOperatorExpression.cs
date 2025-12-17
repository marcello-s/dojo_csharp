#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class BinaryOperatorExpression : IExpression
{
    public IExpression Left { get; private set; }
    public Parser.Token TokenId { get; private set; }
    public IExpression Right { get; private set; }

    public BinaryOperatorExpression(IExpression left, Parser.Token tokenId, IExpression right)
    {
        Left = left;
        TokenId = tokenId;
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
        sb.Append(" ");
        sb.Append(TokenId);
        sb.Append(" ");
        Right.AppendTo(sb);
        sb.Append(")");
    }
}
