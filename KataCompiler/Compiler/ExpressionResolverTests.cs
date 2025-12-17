#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;
using System.Text;
using KataCompiler.Ast;
using KataCompiler.Parser;
using NUnit.Framework;

namespace KataCompiler.Compiler;

[TestFixture]
public class ExpressionResolverTests
{
    [Test]
    public void Resolve_Constants_BinaryOperator()
    {
        const string input = "3.5 + 22.0 / 7.0";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("6.642857142857142"));
    }

    [Test]
    public void Resolve_Constants_BinaryShiftOperator()
    {
        const string input = "0x100 >> 2";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("64"));
    }

    [Test]
    public void Resolve_Constants_BinaryAndOperator()
    {
        const string input = "0x06 & 0x02";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("2"));
    }

    [Test]
    public void Resolve_Constants_BinaryComparisonOperator()
    {
        const string input = "3.5 <= 22.0";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("True"));
    }

    [Test]
    public void Resolve_Constants_UnaryOperator()
    {
        const string input = "~127";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("-128"));
    }

    [Test]
    public void Resolve_Constants_BinaryBooleanOperator()
    {
        const string input = "true && true";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("True"));
    }

    [Test]
    public void Resolve_Constants_UnaryBooleanOperator()
    {
        const string input = "!true";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("False"));
    }

    [Test]
    public void Resolve_Constants_UnaryNumberOperator()
    {
        const string input = "2 + -3";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("-1"));
    }

    [Test]
    public void Resolve_Constants_AllOperators()
    {
        const string input = "(((0x01 << 5) + 32) > 128) || ((0xff | 0xf6) >> 1) <= -2";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("False"));
    }

    [Test]
    public void Resolve_StringConcatenation()
    {
        const string input = "\"foo\" + \"bar\" + \"baz\"";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("foobarbaz"));
    }

    [Test]
    public void Resolve_Function()
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

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "method: Division"
                    + Environment.NewLine
                    + "signature: a, b"
                    + Environment.NewLine
                    + "body: {"
                    + Environment.NewLine
                    + "var: (result = 0)"
                    + Environment.NewLine
                    + ", if: (b Equal3 0){"
                    + Environment.NewLine
                    + "throw: Division by zero!}"
                    + Environment.NewLine
                    + "else: {"
                    + Environment.NewLine
                    + "(result = (a Slash b))}"
                    + Environment.NewLine
                    + ", return: result}"
                    + Environment.NewLine
                    + "call: Division(10, 5)"
            )
        );
    }

    [Test]
    public void Resolve_MethodCall_FromAccessor()
    {
        var input =
            "var this, context;" + Environment.NewLine + "this[ match ]( context[ match ] );";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: this, context"
                    + Environment.NewLine
                    + "call: accessor: this[match]"
                    + Environment.NewLine
                    + "(accessor: context[match]"
                    + Environment.NewLine
                    + ")"
            )
        );
    }

    [Test]
    public void Resolve_IdentifierPart()
    {
        var input =
            "var MyObject = {"
            + Environment.NewLine
            + "MyOtherObject : {"
            + Environment.NewLine
            + "  MyValue : 23 }"
            + Environment.NewLine
            + "};"
            + Environment.NewLine
            + "MyObject.MyOtherObject.MyValue = 67;";

        // symbol: MyObject
        // MyObject := map[MyOtherOject] := map[MyValue] := 23
        // assign: MyObject[MyOtherObject[MyValue]] := 23
        // assign: MyObject[MyOtherObject[MyValue]] := 67

        // Objects are essentially maps. Map entries can be accessed by [] indexer or . notation.
        // assign: MyObject.MyOtherObject.MyValue := 23
        // assign: MyObject.MyOtherObject.MyValue := 67
        // The symbol MyObject can be checked at compile time. The map entries
        // MyOtherObject/MyValue would be created on first access.

        var exprs = Resolve(input);
        var assignExpr = exprs.ElementAt(1) as AssignExpression;
        Assert.That(assignExpr, Is.Not.Null);
        Assert.That(assignExpr.Tag, Is.Not.Null);
        var identifierExpr = (IdentifierExpression)assignExpr.Left;
        Assert.That(identifierExpr.Name, Is.EqualTo("MyObject.MyOtherObject.MyValue"));
    }

    [Test]
    public void Resolve_ObjectLiteral_StringName()
    {
        const string input = "var MyObject = {\"MyOtherObject\" : 23};";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("var: (MyObject = object: MyOtherObject=23)" + Environment.NewLine)
        );
    }

    [Test]
    public void Resolve_DuplicateIdentifier()
    {
        var input = "var a;" + Environment.NewLine + "var a;" + Environment.NewLine;

        var exprs = Resolve(input);
    }

    [Test]
    public void Resolve_IdentifierNotDefined()
    {
        const string input = "a = 7;";

        var exprs = Resolve(input);
        var assignExpr = exprs.ElementAt(0) as AssignExpression;
        Assert.That(assignExpr, Is.Not.Null);
        Assert.That(assignExpr.Tag, Is.Null);
    }

    [Test]
    public void Resolve_While()
    {
        var input =
            "var i;"
            + Environment.NewLine
            + "while (i < 1 + 2) {"
            + Environment.NewLine
            + "  i = 1 + 2;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: i"
                    + Environment.NewLine
                    + "while: (i LessThan 3){"
                    + Environment.NewLine
                    + "(i = 3)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_RemoveUnreachableCodeAfterBreak()
    {
        var input =
            "var i;"
            + Environment.NewLine
            + "if (i < 3) {"
            + Environment.NewLine
            + " break;"
            + Environment.NewLine
            + " ++i;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: i"
                    + Environment.NewLine
                    + "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_RemoveUnreachableCodeAfterContinue()
    {
        var input =
            "var i;"
            + Environment.NewLine
            + "if (i < 3) {"
            + Environment.NewLine
            + " continue;"
            + Environment.NewLine
            + " ++i;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: i"
                    + Environment.NewLine
                    + "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "continue:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_RemoveUnreachableCodeAfterReturn()
    {
        var input =
            "var i;"
            + Environment.NewLine
            + "if (i < 3) {"
            + Environment.NewLine
            + " return;"
            + Environment.NewLine
            + " ++i;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: i"
                    + Environment.NewLine
                    + "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "return: }"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_RemoveUnreachableCodeAfterThrow()
    {
        var input =
            "var i;"
            + Environment.NewLine
            + "if (i < 3) {"
            + Environment.NewLine
            + " throw i;"
            + Environment.NewLine
            + " ++i;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: i"
                    + Environment.NewLine
                    + "if: (i LessThan 3){"
                    + Environment.NewLine
                    + "throw: i}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_Accessor_Set()
    {
        var input = "var myArray;" + Environment.NewLine + "myArray[40 + 2] = 0;";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: myArray"
                    + Environment.NewLine
                    + "(accessor: myArray[42]"
                    + Environment.NewLine
                    + " = 0)"
            )
        );
        var assignExpr = exprs.ElementAt(1) as AssignExpression;
        Assert.That(assignExpr, Is.Not.Null);
        Assert.That(assignExpr.Tag, Is.Not.Null);
    }

    [Test]
    public void Resolve_Accessor_Get()
    {
        var input = "var a, myArray;" + Environment.NewLine + "a = myArray[40 + 2];";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: a, myArray"
                    + Environment.NewLine
                    + "(a = accessor: myArray[42]"
                    + Environment.NewLine
                    + ")"
            )
        );
        var assignExpr = exprs.ElementAt(1) as AssignExpression;
        Assert.That(assignExpr, Is.Not.Null);
        Assert.That(assignExpr.Tag, Is.Not.Null);
    }

    [Test]
    public void Resolve_For_Simple()
    {
        var input = "for (var i = 1 - 1; i < 1 + 2; i++) {" + Environment.NewLine + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "for: init=var: (i = 0)"
                    + Environment.NewLine
                    + " cond=(i LessThan 3) inc=(iPlus2){"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_For_Crazy()
    {
        var input = "for (;;) {" + Environment.NewLine + "  break;" + Environment.NewLine + "}";

        var exprs = Resolve(input);
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
    public void Resolve_For_In()
    {
        var input =
            "var items;"
            + Environment.NewLine
            + "for (var item in items) {"
            + Environment.NewLine
            + " item = 1 + 2;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: items"
                    + Environment.NewLine
                    + "for-in: object=var: (item In items)"
                    + Environment.NewLine
                    + "{"
                    + Environment.NewLine
                    + "(item = 3)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_Try_Catch_Finally()
    {
        var input =
            "var a;"
            + Environment.NewLine
            + "try {"
            + Environment.NewLine
            + "  a = 1 + 2;"
            + Environment.NewLine
            + "} catch (ex) {"
            + Environment.NewLine
            + "  a = 2 + 3;"
            + Environment.NewLine
            + "} finally {"
            + Environment.NewLine
            + "  a = 3 + 4;"
            + Environment.NewLine
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: a"
                    + Environment.NewLine
                    + "try: {(a = 3)}"
                    + Environment.NewLine
                    + "catch: ex {(a = 5)}"
                    + Environment.NewLine
                    + "finally: {(a = 7)}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_Switch_Case_Default()
    {
        var input =
            "var a;"
            + Environment.NewLine
            + "switch (a) {"
            + Environment.NewLine
            + " case 0 :"
            + Environment.NewLine
            + "   a = 1 + 2;"
            + Environment.NewLine
            + "   break;"
            + Environment.NewLine
            + " case 1 :"
            + Environment.NewLine
            + "   a = 2 + 3;"
            + Environment.NewLine
            + "   break;"
            + Environment.NewLine
            + " default :"
            + Environment.NewLine
            + "   a = 3 + 4;"
            + "}";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo(
                "var: a"
                    + Environment.NewLine
                    + "switch: a{"
                    + Environment.NewLine
                    + "case: 0{"
                    + Environment.NewLine
                    + "(a = 3), break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + ", case: 1{"
                    + Environment.NewLine
                    + "(a = 5), break:"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
                    + ", case: (default){"
                    + Environment.NewLine
                    + "(a = 7)}"
                    + Environment.NewLine
                    + "}"
                    + Environment.NewLine
            )
        );
    }

    [Test]
    public void Resolve_New()
    {
        var input = "var regEx;" + Environment.NewLine + "regEx = new RegExp(\"^$\")";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("var: regEx" + Environment.NewLine + "(regEx = new: call: RegExp(^$))")
        );
    }

    [Test]
    public void Resolve_ConditionExpression()
    {
        var input =
            "var a, b;"
            + Environment.NewLine
            + "a = b > 1 + 2"
            + Environment.NewLine
            + " ? 2 + 3"
            + Environment.NewLine
            + " : 3 + 4;";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("var: a, b" + Environment.NewLine + "(a = ((b GreaterThan 3) ? 5 : 7))")
        );
    }

    [Test]
    public void Resolve_ConditionExpression_Static()
    {
        var input =
            "var a;"
            + Environment.NewLine
            + "a = 2 + 3 > 1"
            + Environment.NewLine
            + " ? 2 + 3"
            + Environment.NewLine
            + " : 3 + 4;";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(renderedExprs, Is.EqualTo("var: a" + Environment.NewLine + "(a = 5)"));
    }

    [Test]
    public void Resolve_ArrayLiteral()
    {
        var input = "var a;" + Environment.NewLine + "a = [0, 1, 1 + 1];";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("var: a" + Environment.NewLine + "(a = array: 0, 1, 2)")
        );
    }

    [Test]
    public void Resolve_IllegalExpression_ReportError()
    {
        const string input = " else ";

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Assert.That(
            renderedExprs,
            Is.EqualTo("illegal: Not able to parse 'Else'." + Environment.NewLine)
        );
    }

    [Test, Ignore("because")]
    public void Resolve_jQuery_extract()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var exprs = Resolve(input);
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

        var exprs = Resolve(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    [Test, Ignore("because")]
    public void PrintLinqExpression()
    {
        Func<int, int, double> myFunc = (a, b) =>
        {
            var result = 0.0e1;
            if (b == 0)
            {
                throw new Exception("Division by zero!");
            }
            else
            {
                result = a / b;
            }
            return result;
        };

        System.Linq.Expressions.Expression<Func<int, int, double>> expr = (a, b) => myFunc(a, b);
        Console.WriteLine(expr);
    }

    private static IEnumerable<IExpression> Resolve(string text)
    {
        IEnumerable<IExpression> exprs = null;
        IList<IExpression> resolvedExpr = null;
        var bytes = Encoding.Default.GetBytes(text);

        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var parser = new EcmaScriptParser(new Morpher(new Lexer(sr)));
            exprs = parser.ParseModule();
            var resolver = new ExpressionResolver();

            var scope = new Scope();
            resolvedExpr = new List<IExpression>();
            foreach (var expr in exprs)
            {
                resolvedExpr.Add(resolver.Evaluate(expr, scope));
            }

            if (
                resolver.ErrorReporter.NumberOfErrors > 0
                || resolver.ErrorReporter.NumberOfWarnings > 0
            )
            {
                var errorReport = resolver.ErrorReporter.Render();
                Console.WriteLine(errorReport);
            }
        }

        return resolvedExpr;
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
