#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using NUnit.Framework;

namespace KataDataCompression;

[TestFixture]
public class RleCompressorTest
{
    private const string NyanCatAscii = @"nyan_cat_ascii.txt";
    private const string NyanCatAsciiRle = @"nyan_cat_ascii.rle.txt";
    private const string RleOutputBin = @"rle_output.bin";
    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void CompressAsciiFile_When_ThenCompress()
    {
        var inputPath = Path.Combine(CurrentDirectory, NyanCatAscii);
        var outputPath = Path.Combine(CurrentDirectory, RleOutputBin);
        var input = StreamHelper.Create(inputPath);
        var output = StreamHelper.Create(outputPath);

        var rleCompressor = new RleCompressor();
        rleCompressor.Compress(input, output);

        input.Dispose();
        output.Dispose();
    }

    [Test]
    public void DecompressAsciiFile_When_ThenDecompress()
    {
        var inputPath = Path.Combine(CurrentDirectory, RleOutputBin);
        var outputPath = Path.Combine(CurrentDirectory, NyanCatAsciiRle);
        var input = StreamHelper.Create(inputPath);
        var output = StreamHelper.Create(outputPath);

        var rleCompressor = new RleCompressor();
        rleCompressor.Decompress(input, output);

        input.Dispose();
        output.Dispose();
    }

    [Test]
    public void CompressStringExample_WhenLengthAvg_ThenCompress()
    {
        const string TheString = "aaaaabbccc";
        var expectedOutput = new byte[] { 5, 0x61, 2, 0x62, 3, 0x63 };
        var bytes = Encoding.Default.GetBytes(TheString);
        var input = StreamHelper.Create(bytes);
        var buffer = new byte[6];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Compress(input, output);

        input.Dispose();
        output.Dispose();

        Assert.That(buffer, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void DecompressStringExample_WhenLengthAvg_ThenDecompress()
    {
        var inputBuffer = new byte[] { 5, 0x61, 2, 0x62, 3, 0x63 };
        const string ExpectedString = "aaaaabbccc";
        var input = StreamHelper.Create(inputBuffer);
        var buffer = new byte[ExpectedString.Length];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Decompress(input, output);

        input.Dispose();
        output.Dispose();

        var data = Encoding.Default.GetString(buffer);
        Assert.That(data, Is.EqualTo(ExpectedString));
    }

    [Test]
    public void CompressStringExample_WhenLength1_ThenCompress()
    {
        const string TheString = "a";
        var expectedOutput = new byte[] { 1, 0x61 };
        var bytes = Encoding.Default.GetBytes(TheString);
        var input = StreamHelper.Create(bytes);
        var buffer = new byte[2];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Compress(input, output);

        input.Dispose();
        output.Dispose();

        Assert.That(buffer, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void DecompressStringExample_WhenLength1_ThenDecompress()
    {
        var inputBuffer = new byte[] { 1, 0x61 };
        const string ExpectedString = "a";
        var input = StreamHelper.Create(inputBuffer);
        var buffer = new byte[ExpectedString.Length];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Decompress(input, output);

        input.Dispose();
        output.Dispose();

        var data = Encoding.Default.GetString(buffer);
        Assert.That(data, Is.EqualTo(ExpectedString));
    }

    [Test]
    public void CompressStringExample_WhenLengthExceeded_ThenCompress()
    {
        var theString = new String('a', 320) + "bbccc";
        var expectedOutput = new byte[] { 255, 0x61, 65, 0x61, 2, 0x62, 3, 0x63 };
        var bytes = Encoding.Default.GetBytes(theString);
        var input = StreamHelper.Create(bytes);
        var buffer = new byte[8];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Compress(input, output);

        input.Dispose();
        output.Dispose();

        Assert.That(buffer, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void DecompressStringExample_WhenLengthExceeded_ThenDecompress()
    {
        var inputBuffer = new byte[] { 255, 0x61, 65, 0x61, 2, 0x62, 3, 0x63 };
        var expectedString = new String('a', 320) + "bbccc";
        var input = StreamHelper.Create(inputBuffer);
        var buffer = new byte[expectedString.Length];
        var output = StreamHelper.Create(buffer);

        var rleCompressor = new RleCompressor();
        rleCompressor.Decompress(input, output);

        input.Dispose();
        output.Dispose();

        var data = Encoding.Default.GetString(buffer);
        Assert.That(data, Is.EqualTo(expectedString));
    }
}
