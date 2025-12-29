#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class TryCatchFinallyExpression(
    IExpression tryExprs,
    IExpression catchVariable,
    IExpression catchExprs,
    IExpression? finallyExprs = null
) : IExpression
{
    public IExpression TryExprs { get; private set; } = tryExprs;
    public IExpression CatchVariable { get; private set; } = catchVariable;
    public IExpression CatchExprs { get; private set; } = catchExprs;
    public IExpression? FinallyExprs { get; private set; } = finallyExprs;

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("try: {");
        TryExprs.AppendTo(sb);
        sb.AppendLine("}");
        sb.Append("catch: ");
        CatchVariable.AppendTo(sb);
        sb.Append(" {");
        CatchExprs.AppendTo(sb);
        sb.AppendLine("}");
        if (FinallyExprs != null)
        {
            sb.Append("finally: {");
            FinallyExprs.AppendTo(sb);
            sb.AppendLine("}");
        }
    }
}
