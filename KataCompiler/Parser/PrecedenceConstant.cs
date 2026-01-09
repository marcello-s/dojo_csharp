#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Parser;

// JavaScript operator precedence table
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Operator_Precedence

// Precedence   Operator Type   Associativity   Operators
// 0            Grouping        n/a             (...)
// 1            Property Acc    left-to-right   .  ...[.]
//              new             n/a             new ... (...)
// 2            function call   left-to-right   ...(...)
//              new             left-to-right   new ...
// 3            Increment       n/a             ...++
//              Decrement       n/a             ...--
// 4            Logical NOT     right-to-left   !...
//              Bitwise NOT     right-to-left   ~...
//              Unary Plus      right-to-left   +...
//              Unary Minus     right-to-left   -...
//              typeof          right-to-left   typeof ...
//              void            right-to-left   void ...
//              delete          right-to-left   delete ...
// 5            Multiplication  left-to-right   *
//              Division        left-to-right   /
//              Remainder       left-to-right   %
// 6            Addition        left-to-right   +
//              Subtraction     left-to-right   -
// 7            Bitwise shift   left-to-right   << >> >>>
// 8            Relational      left-to-right   < <= > >=
//              in              left-to-right   in
//              instanceof      left-to-right   instanceof
// 9            Equality        left-to-right   == != === !==
// 10           Bitwise AND     left-to-right   &
// 11           Bitwise XOR     left-to-right   ^
// 12           Bitwise OR      left-to-right   |
// 13           Logical AND     left-to-right   &&
// 14           Logical OR      left-to-right   ||
// 15           Conditional     right-to-left   ... ? ... : ...
// 16           yield           right-to-left   yield ...
// 17           Assignment      right-to-left   = += -= *= /= %= <<= >>= >>>= &= ^= |=
// 18           Spread          n/a             ...
// 19           Comma           left-to-right   ,

static class PrecedenceConstant
{
    public const int Assignment = 1;
    public const int Yield = 2;
    public const int Conditional = 3;
    public const int LogicalOr = 4;
    public const int LogicalAnd = 5;
    public const int BitwiseOr = 6;
    public const int BitwiseXor = 7;
    public const int BitwiseAnd = 8;
    public const int Equality = 9;
    public const int Relational = 10;
    public const int BitwiseShift = 11;
    public const int Sum = 12;
    public const int Product = 13;
    public const int Prefix = 14;
    public const int Postfix = 15;
    public const int Call = 16;
    public const int New = 17;
    public const int Accessor = 18;
}
