#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Ast;

interface IExpressionVisitor<R, S>
{
    R Visit(AccessorExpression expr, S scope);
    R Visit(ArrayLiteralExpression expr, S scope);
    R Visit(AssignExpression expr, S scope);
    R Visit(BinaryOperatorExpression expr, S scope);
    R Visit(BreakExpression expr, S scope);
    R Visit(CallExpression expr, S scope);
    R Visit(CaseExpression expr, S scope);
    R Visit(ConditionalExpression expr, S scope);
    R Visit(ConditionalLoopExpression expr, S scope);
    R Visit(ConstantExpression expr, S scope);
    R Visit(ContinueExpression expr, S scope);
    R Visit(DefinitionExpression expr, S scope);
    R Visit(ForExpression expr, S scope);
    R Visit(ForInExpression expr, S scope);
    R Visit(IdentifierExpression expr, S scope);
    R Visit(IdentifierPartExpression expr, S scope);
    R Visit(IfExpression expr, S scope);
    R Visit(IllegalExpression expr, S scope);
    R Visit(MethodExpression expr, S scope);
    R Visit(NewExpression expr, S scope);
    R Visit(ObjectLiteralExpression expr, S scope);
    R Visit(PostfixExpression expr, S scope);
    R Visit(PrefixExpression expr, S scope);
    R Visit(ReturnExpression expr, S scope);
    R Visit(SequenceExpression expr, S scope);
    R Visit(SwitchExpression expr, S scope);
    R Visit(ThrowExpression expr, S scope);
    R Visit(TryCatchFinallyExpression expr, S scope);
    R Visit(VarExpression expr, S scope);
}
