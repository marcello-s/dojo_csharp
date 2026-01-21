#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Wave;

class WaveDataStream(WaveFormat waveFormat, float[] waveBuffer) : WaveStreamBase(waveFormat)
{
    private int sampleCountTotal;
    private readonly float[] waveBuffer = waveBuffer;
    private int waveBufferIndex;

    protected override int Read(float[]? samples, int offset, int count)
    {
        if (samples == null)
        {
            return 0;
        }

        var channels = WaveFormat.channels;
        var sampleCount = count / channels;
        var index = 0;

        for (var n = 0; n < sampleCount; ++n)
        {
            samples[index + offset] = waveBuffer[waveBufferIndex];

            if (channels == 2)
            {
                samples[index + offset + 1] = waveBuffer[waveBufferIndex + 1];
            }

            index += channels;
            waveBufferIndex += channels;

            if (waveBufferIndex >= waveBuffer.Length)
            {
                waveBufferIndex = 0;
            }
        }

        sampleCountTotal += count;

        var returnValue = count;
        if (sampleCountTotal >= waveBuffer.Length)
        {
            returnValue = 0;
        }

        return returnValue;
    }
}
