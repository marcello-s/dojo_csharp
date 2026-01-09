#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataCompiler.Ast;

namespace KataCompiler.Parser;

class LLParser
{
    private readonly ITokenReader tokenReader;
    private readonly Queue<TokenValue> tokenBuffer;
    private readonly IDictionary<Token, IPrefixParselet> prefixParselets;
    private readonly IDictionary<Token, IInfixParselet> infixParselets;

    public IErrorReporter ErrorReporter { get; private set; }

    protected LLParser(ITokenReader tokenReader)
    {
        this.tokenReader = tokenReader;
        tokenBuffer = new Queue<TokenValue>();
        prefixParselets = new Dictionary<Token, IPrefixParselet>();
        infixParselets = new Dictionary<Token, IInfixParselet>();
        ErrorReporter = new ErrorReporter();
    }

    protected void RegisterParselet(Token tokenId, IPrefixParselet prefixParselet)
    {
        if (!prefixParselets.ContainsKey(tokenId))
        {
            prefixParselets.Add(tokenId, prefixParselet);
        }
    }

    protected void RegisterParselet(Token tokenId, IInfixParselet infixParselet)
    {
        if (!infixParselets.ContainsKey(tokenId))
        {
            infixParselets.Add(tokenId, infixParselet);
        }
    }

    protected void RegisterPrefix(Token tokenId, int precedence)
    {
        RegisterParselet(tokenId, new PrefixOperatorParselet(precedence));
    }

    protected void RegisterPostfix(Token tokenId, int precedence)
    {
        RegisterParselet(tokenId, new PostfixOperatorParselet(precedence));
    }

    protected void RegisterInfixLeft(Token tokenId, int precedence)
    {
        RegisterParselet(tokenId, new BinaryOperatorParselet(precedence, false));
    }

    protected void RegisterInfixRight(Token tokenId, int precedence)
    {
        RegisterParselet(tokenId, new BinaryOperatorParselet(precedence, true));
    }

    public IExpression ParseExpression(int precedence)
    {
        var token = Consume();

        if (!prefixParselets.ContainsKey(token.TokenId))
        {
            ErrorReporter.AddError(token, string.Format("Not able to parse '{0}'.", token.TokenId));
            return new IllegalExpression(
                null,
                null,
                string.Format("Not able to parse '{0}'.", token.TokenId),
                token
            );
        }

        var prefix = prefixParselets[token.TokenId];
        var left = prefix.Parse(this, token);

        while (precedence < GetPrecedence())
        {
            token = Consume();
            var infix = infixParselets[token.TokenId];
            left = infix.Parse(this, left, token);
        }

        return left;
    }

    public IExpression ParseExpression()
    {
        return ParseExpression(0);
    }

    private TokenValue Consume()
    {
        LookAhead(0);
        return tokenBuffer.Dequeue();
    }

    public TokenValue Consume(Token expectedToken)
    {
        var token = LookAhead(0);
        if (token.TokenId != expectedToken)
        {
            ErrorReporter.AddError(
                token,
                string.Format(
                    "Expected token '{0}' and found '{1}' instead.",
                    expectedToken,
                    token.TokenId
                )
            );
            return token;
        }

        return Consume();
    }

    public bool LookAhead(params Token[] tokens)
    {
        for (var i = 0; i < tokens.Length; ++i)
        {
            if (!LookAhead(i).TokenId.Equals(tokens[i]))
            {
                return false;
            }
        }

        return true;
    }

    private TokenValue LookAhead(int distance)
    {
        while (distance >= tokenBuffer.Count)
        {
            tokenBuffer.Enqueue(tokenReader.ReadToken());
        }

        return tokenBuffer.ToArray()[distance];
    }

    private int GetPrecedence()
    {
        var token = LookAhead(0).TokenId;
        return infixParselets.ContainsKey(token) ? infixParselets[token].Precedence : 0;
    }

    public bool Match(params Token[] expectedTokens)
    {
        if (!LookAhead(expectedTokens))
        {
            return false;
        }

        for (var i = 0; i < expectedTokens.Length; ++i)
        {
            Consume();
        }

        return true;
    }

    public virtual IExpression? ParseBlock()
    {
        return null;
    }
}
