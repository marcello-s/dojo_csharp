#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class MethodExpression : IExpression
{
    public string Name { get; private set; }
    public IExpression Args { get; private set; }
    public IExpression Body { get; private set; }

    public MethodExpression(string name, IExpression args, IExpression body)
    {
        Name = name;
        Args = args;
        Body = body;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("method: ");
        sb.AppendLine(string.IsNullOrEmpty(Name) ? "anonymous" : Name);
        sb.Append("signature: ");
        Args.AppendTo(sb);

        sb.AppendLine();
        sb.AppendLine("body: {");
        Body.AppendTo(sb);
        sb.AppendLine("}");
    }
}
