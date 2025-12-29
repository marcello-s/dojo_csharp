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
public class XrefBuilderTests
{
    private Scope scope = null!;

    [SetUp]
    public void Setup()
    {
        scope = new Scope();
    }

    [Test]
    public void BuildXref_CollectConstants()
    {
        var input =
            "var a, b, c, d, e, f;"
            + Environment.NewLine
            + "a = 1 + 2;"
            + Environment.NewLine
            + "b = true || true;"
            + Environment.NewLine
            + "c = \"foo\" + \"bar\""
            + Environment.NewLine
            + "d = 7 - 4;"
            + Environment.NewLine
            + "e = !false;"
            + Environment.NewLine
            + "f = \"fo\" + \"ob\" + \"ar\";";

        var exprs = BuildXref(input);
        Assert.That(scope.Constants.Count(), Is.EqualTo(3));
        var constant = scope.Constants.ElementAt(0);
        Assert.That(constant.Key, Is.Zero);
        Assert.That(constant.Value.Value.ToString(), Is.EqualTo("3"));
        Assert.That(constant.Value.Type, Is.EqualTo(ConstantType.Number));

        constant = scope.Constants.ElementAt(1);
        Assert.That(constant.Key, Is.EqualTo(1));
        Assert.That(constant.Value.Value.ToString(), Is.EqualTo("True"));
        Assert.That(constant.Value.Type, Is.EqualTo(ConstantType.Boolean));

        constant = scope.Constants.ElementAt(2);
        Assert.That(constant.Key, Is.EqualTo(2));
        Assert.That(constant.Value.Value, Is.EqualTo("foobar"));
        Assert.That(constant.Value.Type, Is.EqualTo(ConstantType.String));
    }

    [Test, Ignore("because")]
    public void BuildXref_jQuery_extract()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_extract.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var exprs = BuildXref(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    [Test, Ignore("because")]
    public void BuildXref_jQuery()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("KataCompiler.jquery_1_10_2.js");
        var input = new StreamReader(stream!).ReadToEnd();

        var exprs = BuildXref(input);
        var renderedExprs = RenderExpressions(exprs);
        Console.WriteLine(renderedExprs);
    }

    private IEnumerable<IExpression> BuildXref(string text)
    {
        IList<IExpression>? xrefExpr = null;
        var bytes = Encoding.Default.GetBytes(text);

        using (var ms = new MemoryStream(bytes))
        {
            var sr = new StreamReader(ms);
            var parser = new EcmaScriptParser(new Morpher(new Lexer(sr)));
            var exprs = parser.ParseModule();
            var resolver = new ExpressionResolver();

            IList<IExpression> resolvedExpr = new List<IExpression>();
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

            var xrefBuilder = new XrefBuilder();
            xrefExpr = new List<IExpression>();
            foreach (var expr in resolvedExpr)
            {
                xrefExpr.Add(xrefBuilder.Evaluate(expr, scope));
            }
        }

        return xrefExpr;
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
