#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ConstantExpression : IExpression
{
    private double number;
    private bool boolean;
    private bool typeConverted;
    public string? Constant { get; private set; }
    public ConstantType Type { get; private set; }
    public int Key { get; set; }

    public ConstantExpression(string constant, ConstantType type)
    {
        Constant = constant;
        Type = type;
    }

    public ConstantExpression(double number, ConstantType type = ConstantType.Number)
    {
        this.number = number;
        Type = type;
        typeConverted = true;
    }

    public ConstantExpression(bool boolean, ConstantType type = ConstantType.Boolean)
    {
        this.boolean = boolean;
        Type = type;
        typeConverted = true;
    }

    public R Accept<R, S>(IExpressionVisitor<R, S> visitor, S scope)
    {
        return visitor.Visit(this, scope);
    }

    public void AppendTo(StringBuilder sb)
    {
        switch (Type)
        {
            case ConstantType.Boolean:
                sb.Append(ToBoolean().ToString());
                break;

            case ConstantType.Null:
                sb.Append(Constant);
                break;

            case ConstantType.Number:
                sb.Append(ToNumber().ToString());
                break;

            case ConstantType.RegEx:
                sb.Append(Constant);
                break;

            case ConstantType.String:
                sb.Append(Constant);
                break;
        }
    }

    public double ToNumber()
    {
        if (!typeConverted && !string.IsNullOrEmpty(Constant))
        {
            number = Constant.StartsWith("0x")
                ? Convert.ToUInt32(Constant, 16)
                : double.Parse(Constant);
            typeConverted = true;
        }

        return number;
    }

    public bool ToBoolean()
    {
        if (!typeConverted && !string.IsNullOrEmpty(Constant))
        {
            boolean = bool.Parse(Constant);
            typeConverted = true;
        }

        return boolean;
    }
}

enum ConstantType
{
    Boolean,
    Null,
    Number,
    RegEx,
    String,
}
