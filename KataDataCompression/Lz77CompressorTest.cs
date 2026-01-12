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
public class Lz77CompressorTest
{
    [Test]
    public void TryFindLongestMatch_WhenMatch_ThenGetPosition()
    {
        const string input = "aabcbb";
        const string search = "abc";
        var buffer = Encoding.Default.GetBytes(input);
        var match = Encoding.Default.GetBytes(search);

        byte position;
        var lz77Compressor = new Lz77Compressor();
        for (var i = 0; i < input.Length; i++)
        {
            lz77Compressor.AddToWindow(buffer[i]);
        }

        for (var i = 0; i < match.Length; i++)
        {
            lz77Compressor.AddToLookahead(match[i]);
        }

        var result = lz77Compressor.TryFindLongestMatch(out position);

        Assert.That(result, Is.True);
        Assert.That(position, Is.EqualTo(1));
    }

    [Test]
    public void TryFindLongestMatch_WhenMatch_ThenGetPositionAgain()
    {
        const string input = "aabcbb";
        const string search = "bb";
        var buffer = Encoding.Default.GetBytes(input);
        var match = Encoding.Default.GetBytes(search);

        var lz77Compressor = new Lz77Compressor();
        for (var i = 0; i < input.Length; i++)
        {
            lz77Compressor.AddToWindow(buffer[i]);
        }

        for (var i = 0; i < match.Length; i++)
        {
            lz77Compressor.AddToLookahead(match[i]);
        }

        byte position;
        var result = lz77Compressor.TryFindLongestMatch(out position);

        Assert.That(result, Is.True);
        Assert.That(position, Is.EqualTo(4));
    }

    [Test]
    public void TryFindLongestMatch_WhenNoData_GetFalse()
    {
        const string input = "aabcbb";
        var buffer = Encoding.Default.GetBytes(input);

        var lz77Compressor = new Lz77Compressor();
        for (var i = 0; i < buffer.Length; i++)
        {
            lz77Compressor.AddToWindow(buffer[i]);
        }

        byte position;
        var result = lz77Compressor.TryFindLongestMatch(out position);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Compress_WhenData_ThenCompress()
    {
        const string TheString = "aabcbbabc";
        var bytes = Encoding.Default.GetBytes(TheString);
        var input = StreamHelper.Create(bytes);
        var expectedOutput = new byte[]
        {
            0,
            0,
            0x61,
            1,
            1,
            0,
            0,
            0x62,
            0,
            0,
            0x63,
            2,
            1,
            1,
            1,
            5,
            3,
        };
        var buffer = new byte[expectedOutput.Length];
        var output = StreamHelper.Create(buffer);

        var lz77Compressor = new Lz77Compressor();
        lz77Compressor.Compress(input, output);

        Assert.That(buffer, Is.EqualTo(expectedOutput));
    }
}
