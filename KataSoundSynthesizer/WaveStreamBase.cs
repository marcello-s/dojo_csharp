#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer;

abstract class WaveStreamBase(WaveFormat waveFormat) : IWaveStream
{
    public WaveFormat WaveFormat { get; private set; } = waveFormat;

    public int Read(byte[] samples, int offset, int count)
    {
        var waveBuffer = new WaveBuffer(samples);
        var theOffset = offset / 4;
        var theCount = count / 4;
        var samplesWritten = Read(waveBuffer.FloatBuffer, theOffset, theCount);
        return samplesWritten * 4;
    }

    protected abstract int Read(float[]? samples, int offset, int count);
}
