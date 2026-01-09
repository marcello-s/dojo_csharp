#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class CallExpression(IExpression function, IExpression args) : IExpression
{
    public IExpression Function { get; private set; } = function;
    public IExpression Args { get; private set; } = args;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("call: ");
        Function.AppendTo(sb);
        sb.Append("(");
        Args.AppendTo(sb);
        sb.Append(")");
    }
}
