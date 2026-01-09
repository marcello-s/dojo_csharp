#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

interface IInfixParselet
{
    IExpression Parse(LLParser parser, IExpression left, TokenValue token);
    int Precedence { get; }
}
