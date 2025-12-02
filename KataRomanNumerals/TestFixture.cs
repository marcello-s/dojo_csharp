#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataRomanNumerals
{
    [TestFixture]
    public class TestFixture
    {
        [Test]
        public void TestI()
        {
            Assert.That("I", Is.EqualTo(RomanConverter.Convert("1")));
        }

        [Test]
        public void TestII()
        {
            Assert.That("II", Is.EqualTo(RomanConverter.Convert("2")));
        }

        [Test]
        public void TestIII()
        {
            Assert.That("III", Is.EqualTo(RomanConverter.Convert("3")));
        }

        [Test]
        public void TestIV()
        {
            Assert.That("IV", Is.EqualTo(RomanConverter.Convert("4")));
        }

        [Test]
        public void TestV()
        {
            Assert.That("V", Is.EqualTo(RomanConverter.Convert("5")));
        }

        [Test]
        public void TestVI()
        {
            Assert.That("VI", Is.EqualTo(RomanConverter.Convert("6")));
        }

        [Test]
        public void TestVII()
        {
            Assert.That("VII", Is.EqualTo(RomanConverter.Convert("7")));
        }

        [Test]
        public void TestVIII()
        {
            Assert.That("VIII", Is.EqualTo(RomanConverter.Convert("8")));
        }

        [Test]
        public void TestIX()
        {
            Assert.That("IX", Is.EqualTo(RomanConverter.Convert("9")));
        }

        [Test]
        public void TestX()
        {
            Assert.That("X", Is.EqualTo(RomanConverter.Convert("10")));
        }

        [Test]
        public void TestL()
        {
            Assert.That("L", Is.EqualTo(RomanConverter.Convert("50")));
        }

        [Test]
        public void TestC()
        {
            Assert.That("C", Is.EqualTo(RomanConverter.Convert("100")));
        }

        [Test]
        public void TestD()
        {
            Assert.That("D", Is.EqualTo(RomanConverter.Convert("500")));
        }

        [Test]
        public void TestM()
        {
            Assert.That("M", Is.EqualTo(RomanConverter.Convert("1000")));
        }

        [Test]
        public void TestMCMXCIX()
        {
            Assert.That("MCMXCIX", Is.EqualTo(RomanConverter.Convert("1999")));
        }

        [Test]
        public void TestMMVIII()
        {
            Assert.That("MMVIII", Is.EqualTo(RomanConverter.Convert("2008")));
        }

        /*
        [Test]
        public void TestMMMMMMMMMM()
        {
            Assert.That("MMMMMMMMMM", Is.EqualTo(RomanConverter.Convert("10000")));
        }
        */

        [Test]
        public void Test1()
        {
            Assert.That("1", Is.EqualTo(RomanConverter.Convert("I")));
        }

        [Test]
        public void Test2()
        {
            Assert.That("2", Is.EqualTo(RomanConverter.Convert("II")));
        }

        [Test]
        public void Test3()
        {
            Assert.That("3", Is.EqualTo(RomanConverter.Convert("III")));
        }

        [Test]
        public void Test4()
        {
            Assert.That("4", Is.EqualTo(RomanConverter.Convert("IV")));
        }

        [Test]
        public void Test5()
        {
            Assert.That("5", Is.EqualTo(RomanConverter.Convert("V")));
        }

        [Test]
        public void Test6()
        {
            Assert.That("6", Is.EqualTo(RomanConverter.Convert("VI")));
        }

        [Test]
        public void Test7()
        {
            Assert.That("7", Is.EqualTo(RomanConverter.Convert("VII")));
        }

        [Test]
        public void Test8()
        {
            Assert.That("8", Is.EqualTo(RomanConverter.Convert("VIII")));
        }

        [Test]
        public void Test9()
        {
            Assert.That("9", Is.EqualTo(RomanConverter.Convert("IX")));
        }

        [Test]
        public void Test10()
        {
            Assert.That("10", Is.EqualTo(RomanConverter.Convert("X")));
        }

        [Test]
        public void Test50()
        {
            Assert.That("50", Is.EqualTo(RomanConverter.Convert("L")));
        }

        [Test]
        public void Test100()
        {
            Assert.That("100", Is.EqualTo(RomanConverter.Convert("C")));
        }

        [Test]
        public void Test500()
        {
            Assert.That("500", Is.EqualTo(RomanConverter.Convert("D")));
        }

        [Test]
        public void Test1000()
        {
            Assert.That("1000", Is.EqualTo(RomanConverter.Convert("M")));
        }

        [Test]
        public void Test1999()
        {
            Assert.That("1999", Is.EqualTo(RomanConverter.Convert("MCMXCIX")));
        }

        [Test]
        public void Test2008()
        {
            Assert.That("2008", Is.EqualTo(RomanConverter.Convert("MMVIII")));
        }

        [Test]
        public void Test19()
        {
            Assert.That("19", Is.EqualTo(RomanConverter.Convert("XIX")));
        }
    }
}
