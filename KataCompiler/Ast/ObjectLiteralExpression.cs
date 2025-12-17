#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ObjectLiteralExpression : IExpression
{
    public IExpression Definitions { get; private set; }

    public ObjectLiteralExpression(IExpression definitions)
    {
        Definitions = definitions;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("object: ");
        Definitions.AppendTo(sb);
    }
}
