#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Parser;

static class Punctuation
{
    // Punctuators
    public static IDictionary<string, Token> Punctuators = new Dictionary<string, Token>
    {
        { "(", Token.LeftBracket },
        { ")", Token.RightBracket },
        { "{", Token.LeftBrace },
        { "}", Token.RightBrace },
        { "[", Token.LeftSquareBracket },
        { "]", Token.RightSquareBracket },
        { ";", Token.Semicolon },
        { ",", Token.Comma },
        { ".", Token.Point },
    };

    // Operators
    public static IDictionary<string, Token> Operators = new Dictionary<string, Token>
    {
        { "?", Token.Question },
        { ":", Token.Colon },
        { "~", Token.Tilde },
        { "*", Token.Asterisk },
        { "*=", Token.AsteriskEqual },
        { "%", Token.Percent },
        { "%=", Token.PercentEqual },
        { "^", Token.Carot },
        { "^=", Token.CarotEqual },
        { "/", Token.Slash },
        { "/=", Token.SlashEqual },
        { "&", Token.Ampersand },
        { "&&", Token.Ampersand2 },
        { "&&=", Token.AmpersandEqual },
        { "|", Token.Pipe },
        { "||", Token.Pipe2 },
        { "|=", Token.PipeEqual },
        { "=", Token.Equal },
        { "==", Token.Equal2 },
        { "===", Token.Equal3 },
        { "!", Token.Exclamation },
        { "!=", Token.ExclamationEqual },
        { "!==", Token.ExclamationEqual2 },
        { "+", Token.Plus },
        { "++", Token.Plus2 },
        { "+=", Token.PlusEqual },
        { "-", Token.Minus },
        { "--", Token.Minus2 },
        { "-=", Token.MinusEqual },
        { "<", Token.LessThan },
        { "<<", Token.LessThan2 },
        { "<=", Token.LessThanEqual },
        { "<<=", Token.LessThan2Equal },
        { ">", Token.GreaterThan },
        { ">>", Token.GreaterThan2 },
        { ">=", Token.GreaterThanEqual },
        { ">>=", Token.GreaterThan2Equal },
        { ">>>", Token.GreaterThan3 },
        { ">>>=", Token.GreaterThan3Equal },
    };

    // reserved keywords
    public static IDictionary<string, Token> Keywords = new Dictionary<string, Token>
    {
        { "break", Token.Break },
        { "case", Token.Case },
        { "catch", Token.Catch },
        { "continue", Token.Continue },
        { "debugger", Token.Debugger },
        { "default", Token.Default },
        { "delete", Token.Delete },
        { "do", Token.Do },
        { "else", Token.Else },
        { "finally", Token.Finally },
        { "for", Token.For },
        { "function", Token.Function },
        { "if", Token.If },
        { "in", Token.In },
        { "instanceof", Token.Instanceof },
        { "new", Token.New },
        { "return", Token.Return },
        { "switch", Token.Switch },
        { "this", Token.This },
        { "throw", Token.Throw },
        { "try", Token.Try },
        { "typeof", Token.Typeof },
        { "var", Token.Var },
        { "void", Token.Void },
        { "while", Token.While },
        { "with", Token.With },
        { "true", Token.True },
        { "false", Token.False },
        { "null", Token.Null },
    };

    // future reserved keywords
    public static readonly string[] FutureReservedWord = new[]
    {
        "class",
        "const",
        "enum",
        "export",
        "extends",
        "import",
        "super",
        "implements",
        "interface",
        "let",
        "package",
        "private",
        "protected",
        "public",
        "static",
        "yield",
    };
}
