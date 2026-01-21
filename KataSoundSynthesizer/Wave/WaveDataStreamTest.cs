#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Wave;

[TestFixture]
public class WaveDataStreamTest
{
    private const string Soundbank = @"SoundBank";
    private const string OpenHat003 = @"Open Hat 003.wav";
    private const string EGuitar0001 = @"AKWF_eguitar_0001.wav";
    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    private ManualResetEvent waitHandle = null!;

    [Test]
    public void Play_WaveDataStream_WithFile_ThenPlayed()
    {
        var waveData = FileReader.Read(Path.Combine(CurrentDirectory, Soundbank, OpenHat003));
        if (waveData == null)
        {
            Assert.Fail("waveData is null");
            return;
        }

        var waveBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);

        waitHandle = new ManualResetEvent(false);
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(waveData.SampleRate, waveData.Channels);
        var waveDataStream = new WaveDataStream(waveFormat, waveBuffer);
        var waveOutSynth = new WaveOutSynth();
        waveOutSynth.Stopped += OnWaveOutSynthStopped;

        waveOutSynth.Init(waveDataStream);
        waveOutSynth.Play();

        var timeout = waitHandle.WaitOne(5000);
        waveOutSynth.Stop();
        waveOutSynth.Dispose();
    }

    [Test]
    public void Play_WaveDataStream_WithWaveFormFile_ThenPlayed()
    {
        var waveData = FileReader.Read(Path.Combine(CurrentDirectory, Soundbank, EGuitar0001));
        if (waveData == null)
        {
            Assert.Fail("waveData is null");
            return;
        }

        var waveBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);

        for (var i = 0; i < waveBuffer.Length; ++i)
        {
            Console.WriteLine(i + ";" + waveBuffer[i]);
        }

        waitHandle = new ManualResetEvent(false);
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(waveData.SampleRate, waveData.Channels);
        var waveDataStream = new WaveDataStream(waveFormat, waveBuffer);
        var waveOutSynth = new WaveOutSynth();
        waveOutSynth.Stopped += OnWaveOutSynthStopped;

        waveOutSynth.Init(waveDataStream);
        waveOutSynth.Play();

        var timeout = waitHandle.WaitOne(5000);
        waveOutSynth.Stop();
        waveOutSynth.Dispose();
    }

    private void OnWaveOutSynthStopped(object? sender, StoppedEventData e)
    {
        Console.WriteLine("playback stopped");

        if (waitHandle != null)
        {
            waitHandle.Set();
        }
    }
}
