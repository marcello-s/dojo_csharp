#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Compiler;

class XrefBuilder : BaseExpressionVisitor
{
    public override IExpression Visit(ConstantExpression expr, Scope scope)
    {
        object? value;

        switch (expr.Type)
        {
            case ConstantType.Boolean:
                value = expr.ToBoolean();
                break;
            case ConstantType.Number:
                value = expr.ToNumber();
                break;
            case ConstantType.Null:
            case ConstantType.RegEx:
            case ConstantType.String:
                value = expr.Constant;
                break;
            default:
                value = expr.Constant;
                break;
        }

        if (value != null)
        {
            expr.Key = scope.AddConstant(value, expr.Type);
        }

        return expr;
    }
}
