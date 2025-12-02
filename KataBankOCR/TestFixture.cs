#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataBankOCR
{
    [TestFixture]
    public class TestFixture
    {
        private const string FILE = "TestInput.txt";
        private OcrScanner? scanner;
        private string filePath = string.Empty;

        private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

        [SetUp]
        public void Setup()
        {
            scanner = new OcrScanner();
            filePath = Path.Combine(CurrentDirectory, FILE);
        }

        [TearDown]
        public void Teardown()
        {
            scanner = null;
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
            var entries = scanner?.Scan(filePath, "use case 1", 11);
            if (entries is null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        [Test]
        public void TestScanBrokenEntries()
        {
            var entries = scanner?.Scan(filePath, "use case 3", 3);
            if (entries is null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        [Test]
        public void TestFixBrokenEntries()
        {
            var entries = scanner?.Scan(filePath, "use case 4", 12);
            if (entries is null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
    }
}
