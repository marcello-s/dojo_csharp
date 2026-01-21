#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NAudio.Wave;

namespace KataSoundSynthesizer;

class NAudioPlayWaveFile
{
    private ManualResetEvent waitHandle = null!;

    public void PlayWaveFile(string filename)
    {
        waitHandle = new ManualResetEvent(false);

        var wfr = new WaveFileReader(filename);
        var waveOut = new WaveOut(NAudio.Wave.WaveCallbackInfo.FunctionCallback());
        waveOut.Init(wfr);
        waveOut.PlaybackStopped += WaveOutPlaybackStopped;
        waveOut.Play();
        var timeout = waitHandle.WaitOne(5000);
        waveOut.Stop();
        waveOut.Dispose();
        wfr.Dispose();
    }

    private void WaveOutPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        Console.WriteLine("playback stopped");
        waitHandle.Set();
    }
}
