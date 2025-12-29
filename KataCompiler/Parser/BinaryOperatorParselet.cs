#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class BinaryOperatorParselet(int precedence, bool isRight) : IInfixParselet
{
    public int Precedence { get; private set; } = precedence;
    public bool IsRight { get; set; } = isRight;

    public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
    {
        var right = parser.ParseExpression(Precedence - (IsRight ? 1 : 0));

        //if (IsComparison(token))
        //{
        //    var result = AreComparableExpressions(left, right);
        //}

        //if (IsBoolean(token))
        //{
        //    var result = AreComparableExpressions(left, right);
        //}

        return new BinaryOperatorExpression(left, token.TokenId, right);
    }

    //private static bool IsComparison(TokenValue token)
    //{
    //    return token.TokenId.Equals(Token.LessThan)
    //           || token.TokenId.Equals(Token.GreaterThan);
    //}

    //private static bool IsBoolean(TokenValue token)
    //{
    //    return token.TokenId.Equals(Token.Ampersand2)
    //           || token.TokenId.Equals(Token.Pipe2);
    //}

    //private static bool AreComparableExpressions(IExpression left, IExpression right)
    //{
    //    // all operators that do not evaluate to a boolean result are ok.
    //    // "<>&&||!"

    //    var constLeft = IsBooleanConstant(left);
    //    var constRight = IsBooleanConstant(right);

    //    var leftResult = Walk(left, t => t.Equals(Token.LessThan)
    //                                     || t.Equals(Token.GreaterThan)
    //                                     || t.Equals(Token.Ampersand2)
    //                                     || t.Equals(Token.Pipe2));

    //    var rightResult = Walk(right, t => t.Equals(Token.LessThan)
    //                                       || t.Equals(Token.GreaterThan)
    //                                       || t.Equals(Token.Ampersand2)
    //                                       || t.Equals(Token.Pipe2));

    //    if (constLeft || constRight)
    //    {
    //        return false;
    //    }

    //    return leftResult && rightResult;
    //}

    //private static bool Walk(IExpression node, Func<Token, bool> predicate)
    //{
    //    var result = true;
    //    if (node is BinaryOperatorExpression)
    //    {
    //        var boe = node as BinaryOperatorExpression;
    //        result = predicate(boe.TokenId);
    //        result = result && Walk(boe.Left, predicate);
    //        result = result && Walk(boe.Right, predicate);
    //    }

    //    return result;
    //}

    //private static bool IsBooleanConstant(IExpression node)
    //{
    //    var result = false;
    //    if (node is ConstantExpression)
    //    {
    //        var ce = node as ConstantExpression;
    //        bool value;
    //        result = bool.TryParse(ce.Constant, out value);
    //    }

    //    return result;
    //}
}
