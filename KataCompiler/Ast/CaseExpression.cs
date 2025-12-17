#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class CaseExpression : IExpression
{
    public IExpression CaseExpr { get; private set; }
    public IExpression StmtExpr { get; private set; }
    public bool IsDefault { get; private set; }

    public CaseExpression(IExpression caseExpr, IExpression stmtExpr, bool isDefault = false)
    {
        CaseExpr = caseExpr;
        StmtExpr = stmtExpr;
        IsDefault = isDefault;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        sb.Append("case: ");
        if (IsDefault)
        {
            sb.Append("(default)");
        }

        if (CaseExpr != null)
        {
            CaseExpr.AppendTo(sb);
        }

        sb.AppendLine("{");
        StmtExpr.AppendTo(sb);
        sb.AppendLine("}");
    }
}
