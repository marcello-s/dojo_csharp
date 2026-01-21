#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Wave;

[TestFixture]
public class FileReaderTest
{
    private const string OpenHat003 = @"Open Hat 003.wav";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void Read_WithOpenHat003_ThenFileIsRead()
    {
        var waveData = FileReader.Read(Path.Combine(CurrentDirectory, "SoundBank", OpenHat003));
        Assert.That(waveData, Is.Not.Null);
        if (waveData != null)
        {
            Assert.That(waveData.Channels, Is.EqualTo(2));
            Assert.That(waveData.SampleRate, Is.EqualTo(44100));
            Assert.That(waveData.BitsPerSample, Is.EqualTo(16));
        }
    }

    [Test]
    public void Read_WithOpenHat003_ThenConvertBuffer()
    {
        var waveData = FileReader.Read(Path.Combine(CurrentDirectory, "SoundBank", OpenHat003));
        Assert.That(waveData, Is.Not.Null);

        if (waveData != null)
        {
            var floatBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);
            Assert.That(floatBuffer.Length, Is.EqualTo(66150));
        }
    }
}
