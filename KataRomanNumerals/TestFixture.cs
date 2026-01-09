#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataRomanNumerals;

[TestFixture]
public class TestFixture
{
    [Test]
    public void TestI()
    {
        Assert.That(RomanConverter.Convert("1"), Is.EqualTo("I"));
    }

    [Test]
    public void TestII()
    {
        Assert.That(RomanConverter.Convert("2"), Is.EqualTo("II"));
    }

    [Test]
    public void TestIII()
    {
        Assert.That(RomanConverter.Convert("3"), Is.EqualTo("III"));
    }

    [Test]
    public void TestIV()
    {
        Assert.That(RomanConverter.Convert("4"), Is.EqualTo("IV"));
    }

    [Test]
    public void TestV()
    {
        Assert.That(RomanConverter.Convert("5"), Is.EqualTo("V"));
    }

    [Test]
    public void TestVI()
    {
        Assert.That(RomanConverter.Convert("6"), Is.EqualTo("VI"));
    }

    [Test]
    public void TestVII()
    {
        Assert.That(RomanConverter.Convert("7"), Is.EqualTo("VII"));
    }

    [Test]
    public void TestVIII()
    {
        Assert.That(RomanConverter.Convert("8"), Is.EqualTo("VIII"));
    }

    [Test]
    public void TestIX()
    {
        Assert.That(RomanConverter.Convert("9"), Is.EqualTo("IX"));
    }

    [Test]
    public void TestX()
    {
        Assert.That(RomanConverter.Convert("10"), Is.EqualTo("X"));
    }

    [Test]
    public void TestL()
    {
        Assert.That(RomanConverter.Convert("50"), Is.EqualTo("L"));
    }

    [Test]
    public void TestC()
    {
        Assert.That(RomanConverter.Convert("100"), Is.EqualTo("C"));
    }

    [Test]
    public void TestD()
    {
        Assert.That(RomanConverter.Convert("500"), Is.EqualTo("D"));
    }

    [Test]
    public void TestM()
    {
        Assert.That(RomanConverter.Convert("1000"), Is.EqualTo("M"));
    }

    [Test]
    public void TestMCMXCIX()
    {
        Assert.That(RomanConverter.Convert("1999"), Is.EqualTo("MCMXCIX"));
    }

    [Test]
    public void TestMMVIII()
    {
        Assert.That(RomanConverter.Convert("2008"), Is.EqualTo("MMVIII"));
    }

    /*
    [Test]
    public void TestMMMMMMMMMM()
    {
        Assert.That(RomanConverter.Convert("10000"), Is.EqualTo("MMMMMMMMMM"));
    }
    */

    [Test]
    public void Test1()
    {
        Assert.That(RomanConverter.Convert("I"), Is.EqualTo("1"));
    }

    [Test]
    public void Test2()
    {
        Assert.That(RomanConverter.Convert("II"), Is.EqualTo("2"));
    }

    [Test]
    public void Test3()
    {
        Assert.That(RomanConverter.Convert("III"), Is.EqualTo("3"));
    }

    [Test]
    public void Test4()
    {
        Assert.That(RomanConverter.Convert("IV"), Is.EqualTo("4"));
    }

    [Test]
    public void Test5()
    {
        Assert.That(RomanConverter.Convert("V"), Is.EqualTo("5"));
    }

    [Test]
    public void Test6()
    {
        Assert.That(RomanConverter.Convert("VI"), Is.EqualTo("6"));
    }

    [Test]
    public void Test7()
    {
        Assert.That(RomanConverter.Convert("VII"), Is.EqualTo("7"));
    }

    [Test]
    public void Test8()
    {
        Assert.That(RomanConverter.Convert("VIII"), Is.EqualTo("8"));
    }

    [Test]
    public void Test9()
    {
        Assert.That(RomanConverter.Convert("IX"), Is.EqualTo("9"));
    }

    [Test]
    public void Test10()
    {
        Assert.That(RomanConverter.Convert("X"), Is.EqualTo("10"));
    }

    [Test]
    public void Test50()
    {
        Assert.That(RomanConverter.Convert("L"), Is.EqualTo("50"));
    }

    [Test]
    public void Test100()
    {
        Assert.That(RomanConverter.Convert("C"), Is.EqualTo("100"));
    }

    [Test]
    public void Test500()
    {
        Assert.That(RomanConverter.Convert("D"), Is.EqualTo("500"));
    }

    [Test]
    public void Test1000()
    {
        Assert.That(RomanConverter.Convert("M"), Is.EqualTo("1000"));
    }

    [Test]
    public void Test1999()
    {
        Assert.That(RomanConverter.Convert("MCMXCIX"), Is.EqualTo("1999"));
    }

    [Test]
    public void Test2008()
    {
        Assert.That(RomanConverter.Convert("MMVIII"), Is.EqualTo("2008"));
    }

    [Test]
    public void Test19()
    {
        Assert.That(RomanConverter.Convert("XIX"), Is.EqualTo("19"));
    }
}
