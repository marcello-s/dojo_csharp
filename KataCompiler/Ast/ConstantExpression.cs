#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Ast;

class ConstantExpression : IExpression
{
    private double _number;
    private bool _boolean;
    private bool _typeConverted;
    public string Constant { get; private set; }
    public ConstantType Type { get; private set; }
    public int Key { get; set; }

    public ConstantExpression(string constant, ConstantType type)
    {
        Constant = constant;
        Type = type;
    }

    public ConstantExpression(double number, ConstantType type = ConstantType.Number)
    {
        _number = number;
        Type = type;
        _typeConverted = true;
    }

    public ConstantExpression(bool boolean, ConstantType type = ConstantType.Boolean)
    {
        _boolean = boolean;
        Type = type;
        _typeConverted = true;
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
        if (!_typeConverted)
        {
            _number = Constant.StartsWith("0x")
                ? Convert.ToUInt32(Constant, 16)
                : double.Parse(Constant);
            _typeConverted = true;
        }

        return _number;
    }

    public bool ToBoolean()
    {
        if (!_typeConverted)
        {
            _boolean = bool.Parse(Constant);
            _typeConverted = true;
        }

        return _boolean;
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
