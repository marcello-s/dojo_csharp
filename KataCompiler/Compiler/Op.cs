#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Compiler;

static class Op
{
    // number operators

    private static double Multiply(double left, double right)
    {
        return left * right;
    }

    private static double Modulo(double left, double right)
    {
        return left % right;
    }

    private static double Exor(double left, double right)
    {
        return Convert.ToInt32(left) ^ Convert.ToInt32(right);
    }

    private static double Divide(double left, double right)
    {
        return left / right;
    }

    private static double And(double left, double right)
    {
        return Convert.ToInt32(left) & Convert.ToInt32(right);
    }

    private static double Or(double left, double right)
    {
        return Convert.ToInt32(left) | Convert.ToInt32(right);
    }

    private static bool Equal(double left, double right)
    {
        return left == right;
    }

    private static double Plus(double left, double right)
    {
        return left + right;
    }

    private static double Minus(double left, double right)
    {
        return left - right;
    }

    private static bool LessThan(double left, double right)
    {
        return left < right;
    }

    private static double ShiftLeft(double left, double right)
    {
        return Convert.ToInt32(left) << Convert.ToInt32(right);
    }

    private static bool LessOrEqualThan(double left, double right)
    {
        return left <= right;
    }

    private static bool GreaterThan(double left, double right)
    {
        return left > right;
    }

    private static double ShiftRight(double left, double right)
    {
        return Convert.ToInt32(left) >> Convert.ToInt32(right);
    }

    private static bool GreaterOrEqualThan(double left, double right)
    {
        return left >= right;
    }

    public static readonly IDictionary<Parser.Token, Func<double, double, double>> NumberMap =
        new Dictionary<Parser.Token, Func<double, double, double>>
        {
            { Parser.Token.Asterisk, Multiply },
            { Parser.Token.Percent, Modulo },
            { Parser.Token.Carot, Exor },
            { Parser.Token.Slash, Divide },
            { Parser.Token.Ampersand, And },
            { Parser.Token.Pipe, Or },
            { Parser.Token.Plus, Plus },
            { Parser.Token.Minus, Minus },
            { Parser.Token.LessThan2, ShiftLeft },
            { Parser.Token.GreaterThan2, ShiftRight },
        };

    public static readonly IDictionary<
        Parser.Token,
        Func<double, double, bool>
    > NumberComparisonMap = new Dictionary<Parser.Token, Func<double, double, bool>>
    {
        { Parser.Token.Equal2, Equal },
        { Parser.Token.LessThan, LessThan },
        { Parser.Token.LessThanEqual, LessOrEqualThan },
        { Parser.Token.GreaterThan, GreaterThan },
        { Parser.Token.GreaterThanEqual, GreaterOrEqualThan },
    };

    // unary number operators

    private static double Not(double right)
    {
        return ~Convert.ToInt32(right);
    }

    private static double Minus(double right)
    {
        return -right;
    }

    public static readonly IDictionary<Parser.Token, Func<double, double>> UnaryNumberMap =
        new Dictionary<Parser.Token, Func<double, double>>
        {
            { Parser.Token.Tilde, Not },
            { Parser.Token.Minus, Minus },
        };

    // string operators

    public static string Concat(string left, string right)
    {
        return left + right;
    }

    // boolean operators

    private static bool BooleanAnd(bool left, bool right)
    {
        return left && right;
    }

    private static bool BooleanOr(bool left, bool right)
    {
        return left || right;
    }

    private static bool BooleanEqual(bool left, bool right)
    {
        return left == right;
    }

    public static readonly IDictionary<Parser.Token, Func<bool, bool, bool>> BooleanComparisonMap =
        new Dictionary<Parser.Token, Func<bool, bool, bool>>
        {
            { Parser.Token.Ampersand2, BooleanAnd },
            { Parser.Token.Pipe2, BooleanOr },
            { Parser.Token.Equal2, BooleanEqual },
        };

    // boolean unary operator

    public static bool BooleanNot(bool right)
    {
        return !right;
    }
}
