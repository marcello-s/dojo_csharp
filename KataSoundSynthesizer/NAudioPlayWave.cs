#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NAudio.Wave;

namespace KataSoundSynthesizer;

class NAudioPlayWave
{
    private ManualResetEvent waitHandle = null!;

    public void PlaySineWave()
    {
        var sineWaveProvider = new NAudioSinewaveProvider32(440f, 0.25f);
        sineWaveProvider.SetWaveFormat(16000, 2);

        waitHandle = new ManualResetEvent(false);

        var waveOut = new WaveOut(NAudio.Wave.WaveCallbackInfo.FunctionCallback());
        waveOut.Init(sineWaveProvider);
        waveOut.PlaybackStopped += WaveOutPlaybackStopped;
        waveOut.Play();
        var timeout = waitHandle.WaitOne(5000);
        waveOut.Stop();
        waveOut.Dispose();
    }

    void WaveOutPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        Console.WriteLine("playback stopped");
        waitHandle.Set();
    }
}
