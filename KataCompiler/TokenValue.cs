#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public struct TokenValue
{
    public TextSpan Span { get; set; }
    public Token TokenId { get; set; }
    public string Literal { get; set; }
    public string Message { get; set; }
}
