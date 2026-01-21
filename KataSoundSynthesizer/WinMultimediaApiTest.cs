#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer;

[TestFixture]
public class WinMultimediaApiTest
{
    private ManualResetEvent waitHandle = null!;

    [Test]
    public void WaveOutSynth_GetDeviceCount()
    {
        Console.WriteLine("device count: " + WaveOutSynth.GetDeviceCount());
    }

    [Test, Explicit]
    public void WaveOutSynth_Play()
    {
        waitHandle = new ManualResetEvent(false);
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(16000, 2);
        var waveStream = new SweepWaveStream(waveFormat);
        var waveOutSynth = new WaveOutSynth();
        waveOutSynth.Stopped += OnWaveOutSynthStopped;

        waveOutSynth.Init(waveStream);
        waveOutSynth.Play();

        var timeout = waitHandle.WaitOne(5000);
        waveOutSynth.Stop();
        waveOutSynth.Dispose();
    }

    void OnWaveOutSynthStopped(object? sender, StoppedEventData e)
    {
        Console.WriteLine("playback stopped");

        if (waitHandle != null)
        {
            waitHandle.Set();
        }
    }
}
