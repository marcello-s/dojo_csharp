#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion
        
using System;
using NUnit.Framework;

namespace KataBankOCR
{
    [TestFixture]
    public class TestFixture
    {
        private OcrScanner _scanner;
        private const string FILE = "TestInput.txt";
        private string _filePath;

        private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

        [SetUp]
        public void Setup()
        {
            _scanner = new OcrScanner();
            _filePath = System.IO.Path.Combine(CurrentDirectory, FILE);
        }

        [TearDown]
        public void Teardown()
        {
            _scanner = null;
        }

        [Test]
        public void TestChecksum()
        {
            Assert.That(Entry.IsChecked("000000051"), Is.True);
            Assert.That(Entry.IsChecked("711111111"), Is.True);
            Assert.That(Entry.IsChecked("457508000"), Is.True);
        }

        [Test]
        public void TestScanMarker()
        {
            var entries = _scanner.Scan(_filePath, "use case 1", 11);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        [Test]
        public void TestScanBrokenEntries()
        {
            var entries = _scanner.Scan(_filePath, "use case 3", 3);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        [Test]
        public void TestFixBrokenEntries()
        {
            var entries = _scanner.Scan(_filePath, "use case 4", 12);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
    }
}
