#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;
using System.Text;
using KataCompiler.Ast;
using NUnit.Framework;

namespace KataCompiler.Parser;

[TestFixture]
public class EcmaScriptParserTest
{
    [Test, Ignore("because")]
    public void Parse_Function()
    {
        var input =
            "// divide a by b"
            + Environment.NewLine
            + "function Division(a, b) {"
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
            + "}"
            + Environment.NewLine
            + "Division(10, 5);";

        var exprs = Parse(input);
        Console.WriteLine(RenderExpressions(exprs));
    }

    [Test]
    public void Parse_Comparison()
    {
        const string input = "3 < 5 && true";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("((3 LessThan 5) Ampersand2 True)"));
    }

    [Test]
    public void Parse_Comparison_Commutatively()
    {
        const string input = "true && 3 < 5";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(True Ampersand2 (3 LessThan 5))"));
    }

    [Test]
    public void Parse_Comparison_BadArguments_Warning()
    {
        const string input = "true < false";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    [Test]
    public void Parse_Modulo()
    {
        const string input = "1 * 10 % 2";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("((1 Asterisk 10) Percent 2)"));
    }

    [Test]
    public void Parse_Modulo_Commutatively()
    {
        const string input = "10 % 2 * 1";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("((10 Percent 2) Asterisk 1)"));
    }

    [Test]
    public void Parse_ShiftLeft()
    {
        const string input = "1 + 1 << 4";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("((1 Plus 1) LessThan2 4)"));
    }

    [Test]
    public void Parse_ShiftLeft_Commutatively()
    {
        const string input = "1 << 4 + 1";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(1 LessThan2 (4 Plus 1))"));
    }

    [Test]
    public void Parse_While()
    {
        var input = "while (i < 3) {" + Environment.NewLine + "  i++;" + Environment.NewLine + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "while: (i LessThan 3){" + Environment.NewLine + "(iPlus2)}" + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Do_While()
    {
        var input =
            "do"
            + Environment.NewLine
            + "{"
            + Environment.NewLine
            + "  i++;"
            + Environment.NewLine
            + "} while (i < 3);";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "do-while: (i LessThan 3){"
                    + Environment.NewLine
                    + "(iPlus2)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Break()
    {
        var input = "if (i < 3)" + Environment.NewLine + "  break;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Continue()
    {
        var input = "if (i < 3)" + Environment.NewLine + "  continue;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "continue:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Accessor_Set()
    {
        const string input = "myArray[40 + 2] = 0;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("(accessor: myArray[(40 Plus 2)]" + Environment.NewLine + " = 0)")
        );
    }

    [Test]
    public void Parse_Accessor_Get()
    {
        const string input = "a = myArray[40 + 2];";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("(a = accessor: myArray[(40 Plus 2)]" + Environment.NewLine + ")")
        );
    }

    [Test]
    public void Parse_For_Simple()
    {
        var input = "for (i = 0; i < 3; i++) {" + Environment.NewLine + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "for: init=(i = 0) cond=(i LessThan 3) inc=(iPlus2){"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_For_Crazy()
    {
        var input = "for (;;) {" + Environment.NewLine + "  break;" + Environment.NewLine + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "for: {"
                    + Environment.NewLine
                    + "break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_For_In()
    {
        var input = "for (item in items) {" + Environment.NewLine + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "for-in: object=(item In items){" + Environment.NewLine + "}" + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Try_Catch()
    {
        var input =
            "try {"
            + Environment.NewLine
            + "  willEventuallyFail();"
            + Environment.NewLine
            + "} catch (ex) {"
            + Environment.NewLine
            + "  console.log(ex);"
            + Environment.NewLine
            + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "try: {call: willEventuallyFail()}"
                    + Environment.NewLine
                    + "catch: ex {call: console.log(ex)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Try_Catch_invalid()
    {
        var input =
            "try {"
            + Environment.NewLine
            + "  willEventuallyFail();"
            + Environment.NewLine
            + "} catch () {"
            + Environment.NewLine
            + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "try: {call: willEventuallyFail()}"
                    + Environment.NewLine
                    + "catch: illegal: Not able to parse 'RightBracket'."
                    + Environment.NewLine
                    + " {}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Try_Catch_Finally()
    {
        var input =
            "try {"
            + Environment.NewLine
            + "  willEventuallyFail();"
            + Environment.NewLine
            + "} catch (ex) {"
            + Environment.NewLine
            + "  console.log(ex);"
            + Environment.NewLine
            + "} finally {"
            + Environment.NewLine
            + "  console.log(\"Exception handled.\");"
            + Environment.NewLine
            + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "try: {call: willEventuallyFail()}"
                    + Environment.NewLine
                    + "catch: ex {call: console.log(ex)}"
                    + Environment.NewLine
                    + "finally: {call: console.log(Exception handled.)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Switch_Case()
    {
        var input =
            "switch (a) {"
            + Environment.NewLine
            + " case 0 :"
            + Environment.NewLine
            + "   console.log(0);"
            + Environment.NewLine
            + "   break;"
            + Environment.NewLine
            + " case 1 :"
            + Environment.NewLine
            + "   console.log(1);"
            + Environment.NewLine
            + "   break;"
            + Environment.NewLine
            + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "switch: a{"
                    + Environment.NewLine
                    + "case: 0{"
                    + Environment.NewLine
                    + "call: console.log(0), break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + ", case: 1{"
                    + Environment.NewLine
                    + "call: console.log(1), break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_Switch_Case_Default()
    {
        var input =
            "switch (a) {"
            + Environment.NewLine
            + " case 0 :"
            + Environment.NewLine
            + "   console.log(0);"
            + Environment.NewLine
            + "   break;"
            + Environment.NewLine
            + " default :"
            + Environment.NewLine
            + "   console.log(1);"
            + Environment.NewLine
            + "}";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "switch: a{"
                    + Environment.NewLine
                    + "case: 0{"
                    + Environment.NewLine
                    + "call: console.log(0), break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + ", case: (default){"
                    + Environment.NewLine
                    + "call: console.log(1)}"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Parse_New()
    {
        const string input = "regEx = new RegExp(\"^$\")";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(regEx = new: call: RegExp(^$))"));
    }

    [Test]
    public void Parse_Delete()
    {
        const string input = "delete myArray[7];";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("(Delete accessor: myArray[7]" + Environment.NewLine + ")")
        );
    }

    [Test]
    public void Parse_TypeOf()
    {
        const string input = "typeof myStuff;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(Typeof myStuff)"));
    }

    [Test]
    public void Parse_Void()
    {
        const string input = "void (0);";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(Void 0)"));
    }

    [Test]
    public void Parse_InstanceOf()
    {
        const string input = "c instanceof String;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(c Instanceof String)"));
    }

    [Test]
    public void Parse_Object_Simple()
    {
        const string input = "a = {};";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(a = object: )"));
    }

    [Test]
    public void Parse_ObjectLiteral_Advanced()
    {
        const string input = "a = {prop: \"unicorn\", f: function() {}};";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "(a = object: prop=unicorn, f=method: anonymous"
                    + Environment.NewLine
                    + "signature: "
                    + Environment.NewLine
                    + "body: {"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + ")"
            )
        );
    }

    [Test]
    public void Parse_ObjectLiteral_StringName()
    {
        const string input = "a = {\"prop\": \"unicorn\"};";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(a = object: prop=unicorn)"));
    }

    [Test]
    public void Parse_Array_Simple()
    {
        const string input = "a = [];";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(a = array: )"));
    }

    [Test]
    public void Parse_Array_Advanced()
    {
        const string input = "a = [0, 1, 1 + 1];";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(a = array: 0, 1, (1 Plus 1))"));
    }

    [Test]
    public void Parse_IdentifierPart()
    {
        const string input = "console.log(0);";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("call: console.log(0)"));
    }

    [Test]
    public void Parse_IdentifierPart_Advanced()
    {
        const string input = "MyObject.MyOtherObject.MyValue = 67;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(MyObject.MyOtherObject.MyValue = 67)"));
    }

    [Test]
    public void Parse_TernaryCondition()
    {
        var input = "a = b > 1" + Environment.NewLine + " ? 2" + Environment.NewLine + " : 3;";

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("(a = ((b GreaterThan 1) ? 2 : 3))"));
    }

    [Test, Ignore("because")]
    public void Parse_ExhaustiveTest()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    [Test, Ignore("because")]
    public void Parse_jQuery()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_1_10_2.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var exprs = Parse(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    private static IEnumerable<IExpression> Parse(string text)
    {
        IEnumerable<IExpression> exprs = null;
        var bytes = Encoding.Default.GetBytes(text);

        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var parser = new EcmaScriptParser(new Morpher(new Lexer(sr)));
            exprs = parser.ParseModule();

            //if (parser.ErrorReporter.NumberOfErrors > 0)
            //{
            //    ms.Position = 0;
            //    var errorReport = parser.ErrorReporter.Render(sr);
            //    Console.WriteLine(errorReport);
            //}
        }

        return exprs;
    }

    private static string RenderExpressions(IEnumerable<IExpression> exprs)
    {
        var sb = new StringBuilder();
        foreach (var expr in exprs)
        {
            expr.AppendTo(sb);
        }

        return sb.ToString();
    }
}
