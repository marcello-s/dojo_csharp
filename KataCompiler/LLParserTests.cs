#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using NUnit.Framework;

namespace KataCompiler;

[TestFixture]
public class LLParserTests
{
    [Test]
    public void LLParse()
    {
        // a+b
        var tokens = new List<TokenValue>
        {
            new TokenValue { TokenId = Token.Identifier, Literal = "a" },
            new TokenValue { TokenId = Token.Plus },
            new TokenValue { TokenId = Token.Identifier, Literal = "b" },
        };

        var parser = new TestParser(tokens.GetEnumerator());
        var expression = parser.ParseExpression();

        var sb = new StringBuilder();
        expression.AppendTo(sb);
        Console.WriteLine(sb);
    }

    [Test]
    public void SimpleTest()
    {
        Test("a * b + c", "((a Asterisk b) Plus c)");
        Test("a * (b + c)", "(a Asterisk (b Plus c))");
        Test("a + b * c", "(a Plus (b Asterisk c))");
        Test("(a + b) * c", "((a Plus b) Asterisk c)");
        Test("-a / b", "((Minusa) Slash b)");
    }

    [Test]
    public void ExhaustiveTest()
    {
        // function call
        Test("a()", "a()");
        Test("a(b)", "a(b)");
        Test("a(b, c)", "a(b, c)");
        Test("a(b)(c)", "a(b)(c)");
        Test("a(b) + c(d)", "(a(b) Plus c(d))");
        Test("a(b ? c : d, e + f)", "a((b ? c : d), (e Plus f))");

        // unary precedence
        Test("~!-+a", "(Tilde(Exclamation(Minus(Plusa))))");
        Test("a!!!", "(((aExclamation)Exclamation)Exclamation)");

        // unary and binary predecence
        Test("-a * b", "((Minusa) Asterisk b)");
        Test("!a + b", "((Exclamationa) Plus b)");
        Test("~a ^ b", "((Tildea) Carot b)");
        Test("-a!", "(Minus(aExclamation))");
        Test("!a!", "(Exclamation(aExclamation))");

        // binary precedence
        Test(
            "a = b + c * d ^ e - f / g",
            "(a = ((b Plus (c Asterisk (d Carot e))) Minus (f Slash g)))"
        );

        // binary associativity
        Test("a = b = c", "(a = (b = c))");
        Test("a + b - c", "((a Plus b) Minus c)");
        Test("a * b / c", "((a Asterisk b) Slash c)");
        Test("a ^ b ^ c", "(a Carot (b Carot c))");

        // conditional operator
        Test("a ? b : c ? d : e", "(a ? b : (c ? d : e))");
        Test("a ? b ? c : d : e", "(a ? (b ? c : d) : e)");
        Test("a + b ? c * d : e / f", "((a Plus b) ? (c Asterisk d) : (e Slash f))");

        // grouping
        Test("a + (b + c) + d", "((a Plus (b Plus c)) Plus d)");
        Test("a ^ (b + c)", "(a Carot (b Plus c))");
        Test("(!a)!", "((Exclamationa)Exclamation)");
    }

    private static void Test(string input, string expectedParsedResult)
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(input);
        var parser = new TestParser(tokens.GetEnumerator());
        var expression = parser.ParseExpression();
        var sb = new StringBuilder();
        expression.AppendTo(sb);

        var prettyPrint = sb.ToString();
        var result = prettyPrint.Equals(expectedParsedResult) ? "PASS" : "FAIL";

        Console.WriteLine("'{0}';'{1}' ->'{2}'", input, prettyPrint, result);
    }

    class Lexer
    {
        private static readonly IDictionary<char, Token> Punctuation = new Dictionary<char, Token>
        {
            { '(', Token.LeftBracket },
            { ')', Token.RightBracket },
            { '+', Token.Plus },
            { '-', Token.Minus },
            { '*', Token.Asterisk },
            { '/', Token.Slash },
            { '=', Token.Equal },
            { '?', Token.Question },
            { '!', Token.Exclamation },
            { '~', Token.Tilde },
            { '^', Token.Carot },
            { ':', Token.Colon },
            { ',', Token.Comma },
        };

        public IEnumerable<TokenValue> Tokenize(string input)
        {
            foreach (var c in input)
            {
                if (Punctuation.ContainsKey(c))
                {
                    yield return new TokenValue
                    {
                        TokenId = Punctuation[c],
                        Literal = c.ToString(),
                    };
                }
                else if (char.IsLetter(c))
                {
                    yield return new TokenValue
                    {
                        TokenId = Token.Identifier,
                        Literal = c.ToString(),
                    };
                }
            }
        }
    }

    class LLParser
    {
        private readonly IEnumerator<TokenValue> _tokens;
        private readonly Queue<TokenValue> _tokenBuffer;
        private readonly IDictionary<Token, IPrefixParselet> _prefixParselets;
        private readonly IDictionary<Token, IInfixParselet> _infixParselets;

        public LLParser(IEnumerator<TokenValue> tokens)
        {
            _tokens = tokens;
            _tokenBuffer = new Queue<TokenValue>();
            _prefixParselets = new Dictionary<Token, IPrefixParselet>();
            _infixParselets = new Dictionary<Token, IInfixParselet>();
        }

        protected void RegisterParselet(Token tokenId, IPrefixParselet prefixParselet)
        {
            if (!_prefixParselets.ContainsKey(tokenId))
            {
                _prefixParselets.Add(tokenId, prefixParselet);
            }
        }

        protected void RegisterParselet(Token tokenId, IInfixParselet infixParselet)
        {
            if (!_infixParselets.ContainsKey(tokenId))
            {
                _infixParselets.Add(tokenId, infixParselet);
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

            if (!_prefixParselets.ContainsKey(token.TokenId))
            {
                throw new Exception(string.Format("Not able to parse '{0}'.'", token.TokenId));
            }

            var prefix = _prefixParselets[token.TokenId];
            var left = prefix.Parse(this, token);

            while (precedence < GetPrecedence())
            {
                token = Consume();
                var infix = _infixParselets[token.TokenId];
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
            return _tokenBuffer.Dequeue();
        }

        public TokenValue Consume(Token expectedToken)
        {
            var token = LookAhead(0);
            if (token.TokenId != expectedToken)
            {
                throw new Exception(
                    string.Format(
                        "Expected token '{0}' and found '{1}' instead.",
                        expectedToken,
                        token.TokenId
                    )
                );
            }

            return Consume();
        }

        private TokenValue LookAhead(int distance)
        {
            while (distance >= _tokenBuffer.Count)
            {
                if (_tokens.MoveNext())
                {
                    _tokenBuffer.Enqueue(_tokens.Current);
                }
                else
                {
                    return new TokenValue { TokenId = Token.EOF };
                }
            }

            return _tokenBuffer.Peek();
        }

        private int GetPrecedence()
        {
            var token = LookAhead(0).TokenId;
            return _infixParselets.ContainsKey(token) ? _infixParselets[token].Precedence : 0;
        }

        public bool Match(Token expectedToken)
        {
            var token = LookAhead(0);
            if (token.TokenId != expectedToken)
            {
                return false;
            }

            Consume();
            return true;
        }
    }

    class TestParser : LLParser
    {
        public TestParser(IEnumerator<TokenValue> tokens)
            : base(tokens)
        {
            RegisterParselet(Token.Identifier, new IdentifierParselet());
            RegisterParselet(Token.Equal, new AssignParselet());
            RegisterParselet(Token.Question, new ConditionalParselet());
            RegisterParselet(Token.LeftBracket, new GroupParselet());
            RegisterParselet(Token.LeftBracket, new CallParselet());

            RegisterPrefix(Token.Plus, PrecedenceConstant.Prefix);
            RegisterPrefix(Token.Minus, PrecedenceConstant.Prefix);
            RegisterPrefix(Token.Tilde, PrecedenceConstant.Prefix);
            RegisterPrefix(Token.Exclamation, PrecedenceConstant.Prefix);
            RegisterPostfix(Token.Exclamation, PrecedenceConstant.Postfix);

            RegisterInfixLeft(Token.Plus, PrecedenceConstant.Sum);
            RegisterInfixLeft(Token.Minus, PrecedenceConstant.Sum);
            RegisterInfixLeft(Token.Asterisk, PrecedenceConstant.Product);
            RegisterInfixLeft(Token.Slash, PrecedenceConstant.Product);
            RegisterInfixRight(Token.Carot, PrecedenceConstant.Exponent);
        }
    }

    interface IPrefixParselet
    {
        IExpression Parse(LLParser parser, TokenValue token);
    }

    interface IInfixParselet
    {
        IExpression Parse(LLParser parser, IExpression left, TokenValue token);
        int Precedence { get; }
    }

    interface IExpression
    {
        void AppendTo(StringBuilder sb);
    }

    // simple identifiers e.g. 'a'
    class IdentifierExpression : IExpression
    {
        private string Name { get; set; }

        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append(Name);
        }
    }

    // unary prefix e.g. '+,-,~,!'
    class PrefixExpression : IExpression
    {
        public Token TokenId { get; set; }
        public IExpression Right { get; set; }

        public PrefixExpression(Token tokenId, IExpression right)
        {
            TokenId = tokenId;
            Right = right;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("(");
            sb.Append(TokenId);
            Right.AppendTo(sb);
            sb.Append(")");
        }
    }

    // binary operator e.g. '1 + 2'
    class BinaryOperatorExpression : IExpression
    {
        public IExpression Left { get; set; }
        public Token TokenId { get; set; }
        public IExpression Right { get; set; }

        public BinaryOperatorExpression(IExpression left, Token tokenId, IExpression right)
        {
            Left = left;
            TokenId = tokenId;
            Right = right;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("(");
            Left.AppendTo(sb);
            sb.Append(" ");
            sb.Append(TokenId);
            sb.Append(" ");
            Right.AppendTo(sb);
            sb.Append(")");
        }
    }

    // right operand to binary operator
    class PostfixExpression : IExpression
    {
        public IExpression Left { get; set; }
        public Token TokenId { get; set; }

        public PostfixExpression(IExpression left, Token tokenId)
        {
            Left = left;
            TokenId = tokenId;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("(");
            Left.AppendTo(sb);
            sb.Append(TokenId);
            sb.Append(")");
        }
    }

    class ConditionalExpression : IExpression
    {
        public IExpression Condition { get; set; }
        public IExpression TruthyBranch { get; set; }
        public IExpression FalsyBranch { get; set; }

        public ConditionalExpression(
            IExpression condition,
            IExpression truthyBranch,
            IExpression falsyBranch
        )
        {
            Condition = condition;
            TruthyBranch = truthyBranch;
            FalsyBranch = falsyBranch;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("(");
            Condition.AppendTo(sb);
            sb.Append(" ? ");
            TruthyBranch.AppendTo(sb);
            sb.Append(" : ");
            FalsyBranch.AppendTo(sb);
            sb.Append(")");
        }
    }

    class AssignExpression : IExpression
    {
        public IExpression Left { get; set; }
        public IExpression Right { get; set; }

        public AssignExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("(");
            Left.AppendTo(sb);
            sb.Append(" = ");
            Right.AppendTo(sb);
            sb.Append(")");
        }
    }

    class CallExpression : IExpression
    {
        public IExpression Function { get; set; }
        public IEnumerable<IExpression> Arguments { get; set; }

        public CallExpression(IExpression function, IEnumerable<IExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public void AppendTo(StringBuilder sb)
        {
            Function.AppendTo(sb);
            sb.Append("(");
            for (var i = 0; i < Arguments.Count(); ++i)
            {
                Arguments.ElementAt(i).AppendTo(sb);
                if (i < Arguments.Count() - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
        }
    }

    static class PrecedenceConstant
    {
        public const int Assignment = 1;
        public const int Conditional = 2;
        public const int Sum = 3;
        public const int Product = 4;
        public const int Exponent = 5;
        public const int Prefix = 6;
        public const int Postfix = 7;
        public const int Call = 8;
    }

    class IdentifierParselet : IPrefixParselet
    {
        public IExpression Parse(LLParser parser, TokenValue token)
        {
            return new IdentifierExpression(token.Literal);
        }
    }

    class PrefixOperatorParselet : IPrefixParselet
    {
        public int Precedence { get; set; }

        public PrefixOperatorParselet(int precedence)
        {
            Precedence = precedence;
        }

        public IExpression Parse(LLParser parser, TokenValue token)
        {
            var right = parser.ParseExpression(Precedence);
            return new PrefixExpression(token.TokenId, right);
        }
    }

    class GroupParselet : IPrefixParselet
    {
        public IExpression Parse(LLParser parser, TokenValue token)
        {
            var expr = parser.ParseExpression();
            parser.Consume(Token.RightBracket);
            return expr;
        }
    }

    class BinaryOperatorParselet : IInfixParselet
    {
        public int Precedence { get; set; }
        public bool IsRight { get; set; }

        public BinaryOperatorParselet(int precedence, bool isRight)
        {
            Precedence = precedence;
            IsRight = isRight;
        }

        public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
        {
            var right = parser.ParseExpression(Precedence - (IsRight ? 1 : 0));
            return new BinaryOperatorExpression(left, token.TokenId, right);
        }
    }

    class PostfixOperatorParselet : IInfixParselet
    {
        public int Precedence { get; set; }

        public PostfixOperatorParselet(int precedence)
        {
            Precedence = precedence;
        }

        public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
        {
            return new PostfixExpression(left, token.TokenId);
        }
    }

    class ConditionalParselet : IInfixParselet
    {
        public int Precedence { get; set; }

        public ConditionalParselet()
        {
            Precedence = PrecedenceConstant.Conditional;
        }

        public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
        {
            var truthyBranch = parser.ParseExpression();
            parser.Consume(Token.Colon);
            var falsyBranch = parser.ParseExpression();
            return new ConditionalExpression(left, truthyBranch, falsyBranch);
        }
    }

    class AssignParselet : IInfixParselet
    {
        public int Precedence { get; set; }

        public AssignParselet()
        {
            Precedence = PrecedenceConstant.Assignment;
        }

        public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
        {
            var right = parser.ParseExpression(PrecedenceConstant.Assignment - 1);

            if (!(left is IdentifierExpression))
            {
                throw new Exception(
                    "The left-hand side of an assignment must be an IdentifierExpression"
                );
            }

            return new AssignExpression(left, right);
        }
    }

    class CallParselet : IInfixParselet
    {
        public int Precedence { get; set; }

        public CallParselet()
        {
            Precedence = PrecedenceConstant.Call;
        }

        public IExpression Parse(LLParser parser, IExpression left, TokenValue token)
        {
            var args = new List<IExpression>();

            if (!parser.Match(Token.RightBracket))
            {
                do
                {
                    args.Add(parser.ParseExpression());
                } while (parser.Match(Token.Comma));
                parser.Consume(Token.RightBracket);
            }

            return new CallExpression(left, args);
        }
    }
}
