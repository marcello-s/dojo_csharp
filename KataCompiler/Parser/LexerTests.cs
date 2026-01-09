#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace KataCompiler.Parser;

[TestFixture]
public class LexerTests
{
    [Test]
    public void Lexer_SimpleTest()
    {
        var input =
            "function Division(a, b) {"
            + Environment.NewLine
            + "  var result = 0.0e1;"
            + Environment.NewLine
            + "  if (b === 0) {"
            + Environment.NewLine
            + "    throw \"Division by zero!\";"
            + Environment.NewLine
            + "  }"
            + Environment.NewLine
            + "  else {"
            + Environment.NewLine
            + "    result = a / b;"
            + Environment.NewLine
            + "  }"
            + Environment.NewLine
            + "  return result;"
            + Environment.NewLine
            + "}";
        var renderedTokens = Scan(input);
    }

    [Test, Ignore("because")]
    public void Lexer_ExhaustiveTest()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();
        var renderedTokens = Scan(input);
        Console.WriteLine(renderedTokens);
    }

    [Test, Ignore("because")]
    public void Lexer_jQuery()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_1_10_2.js");
        var input = new StreamReader(stream!).ReadToEnd();
        var renderedTokens = Scan(input);
        Console.WriteLine(renderedTokens);
    }

    [Test]
    public void Lexer_EscapedString()
    {
        const string input = "The \"quick brown fox\" jumps over the \"\\\"lazy\\\"\" dog";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'The' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'StringLiteral' lit:'quick brown fox' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Identifier' lit:'jumps' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Identifier' lit:'over' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Identifier' lit:'the' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'StringLiteral' lit:'\"lazy\"' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Identifier' lit:'dog' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_SingleQuotedString()
    {
        const string input = "'single \"quoted\" string'  'str\ttabbed'";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'StringLiteral' lit:'single \"quoted\" string' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:'  ' "
                    + Environment.NewLine
                    + "tok:'StringLiteral' lit:'str	tabbed' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_Number()
    {
        const string input = "123 45e-6 78.90 12E+3 0xabcdff";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'NumberLiteral' lit:'123' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'NumberLiteral' lit:'45e-6' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'NumberLiteral' lit:'78.90' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'NumberLiteral' lit:'12E+3' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'NumberLiteral' lit:'0xabcdff' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_Number_Scientific()
    {
        const string input = "134e+2+5";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'NumberLiteral' lit:'134e+2' "
                    + Environment.NewLine
                    + "tok:'Plus' lit:'+' "
                    + Environment.NewLine
                    + "tok:'NumberLiteral' lit:'5' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_Comment()
    {
        var input =
            " /* 123.5 */// -line-comment-"
            + Environment.NewLine
            + " /* -illegal"
            + Environment.NewLine
            + " ";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'BlockComment' lit:'/* 123.5 */' "
                    + Environment.NewLine
                    + "tok:'LineComment' lit:'// -line-comment-' "
                    + Environment.NewLine
                    + "tok:'NewLine' lit:'\r"
                    + "' "
                    + Environment.NewLine
                    + "tok:'NewLine' lit:'\n"
                    + "' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Illegal' lit:'/* -illegal"
                    + Environment.NewLine
                    + " ' msg:'Unterminated block comment.'"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_RegExLiteral()
    {
        const string input = "rdashAlpha = /-([\\da-z])/gi";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'rdashAlpha' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Equal' lit:'=' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'RegEx' lit:'/-([\\da-z])/gi' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_RegExLiteral_EscapedSlash()
    {
        const string input = @"rvalidescape = /\\(?:[""\\\/bfnrt]|u[\da-fA-F]{4})/g";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'rvalidescape' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Equal' lit:'=' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'RegEx' lit:'"
                    + @"/\\(?:[""\\\/bfnrt]|u[\da-fA-F]{4})/g' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_RegExLiteral_SquareBracket()
    {
        const string input = "core_pnum = /[+-]?(?:\\d*\\.|)\\d+(?:[eE][+-]?\\d+|)/";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'core_pnum' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Equal' lit:'=' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'RegEx' lit:'/[+-]?(?:\\d*\\.|)\\d+(?:[eE][+-]?\\d+|)/' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_RegExLiteral_AsPartOfObjectLiteral()
    {
        const string input = "r = /\\S+/g, ";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'r' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Equal' lit:'=' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'RegEx' lit:'/\\S+/g' "
                    + Environment.NewLine
                    + "tok:'Comma' lit:',' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Lexer_RegExLiteral_ShortExpr()
    {
        const string input = "r = /^-ms-/,";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'r' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'Equal' lit:'=' "
                    + Environment.NewLine
                    + "tok:'WhiteSpace' lit:' ' "
                    + Environment.NewLine
                    + "tok:'RegEx' lit:'/^-ms-/' "
                    + Environment.NewLine
                    + "tok:'Comma' lit:',' "
                    + Environment.NewLine
            )
        );
    }

    [Test, Ignore("because")]
    public void Lexer_Stuff()
    {
        const string input = "rescape = /'|\\\\/g,";

        var renderedTokens = Scan(input);
        Console.WriteLine(renderedTokens);
    }

    [Test]
    public void Lexer_IdentifierPart()
    {
        const string input = "console.log";
        var renderedTokens = Scan(input);
        Assert.That(
            renderedTokens,
            Is.EqualTo(
                "tok:'Identifier' lit:'console' "
                    + Environment.NewLine
                    + "tok:'Point' lit:'.' "
                    + Environment.NewLine
                    + "tok:'Identifier' lit:'log' "
                    + Environment.NewLine
            )
        );
    }

    private static string Scan(string input)
    {
        var sb = new StringBuilder();
        var bytes = Encoding.Default.GetBytes(input);

        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var lexer = new Lexer(sr);

            var token = lexer.ReadToken();
            while (token.TokenId != Token.EOF)
            {
                //Console.WriteLine(FormatTokenValue(token));
                sb.AppendLine(FormatTokenValue(token));
                token = lexer.ReadToken();
            }
        }

        return sb.ToString();
    }

    private static string FormatTokenValue(TokenValue tokenValue)
    {
        var sb = new StringBuilder();

        sb.AppendFormat("tok:'{0}' ", tokenValue.TokenId);
        //sb.AppendFormat("src:'{0}' ", tokenValue.SrcPosition.Text);

        if (!string.IsNullOrEmpty(tokenValue.Literal))
        {
            sb.AppendFormat("lit:'{0}' ", tokenValue.Literal);
        }

        if (!string.IsNullOrEmpty(tokenValue.Message))
        {
            sb.AppendFormat("msg:'{0}'", tokenValue.Message);
        }

        return sb.ToString();
    }
}
