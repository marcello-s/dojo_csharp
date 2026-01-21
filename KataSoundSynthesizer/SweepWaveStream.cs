#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer;

class SweepWaveStream(WaveFormat waveFormat) : WaveStreamBase(waveFormat)
{
    private int sample;
    private int sampleCountTotal;
    private const float PI2 = (float)(2 * Math.PI);
    private const float amplitude = 0.25f;
    private const float frequency = 440f;
    private const float Sweep = 0.01f;
    private float sweepTotal;

    protected override int Read(float[]? samples, int offset, int count)
    {
        var sampleRate = WaveFormat.sampleRate;
        var channels = WaveFormat.channels;
        var index = 0;

        if (samples != null)
        {
            for (var n = 0; n < count / channels; ++n)
            {
                var f = (frequency + sweepTotal) * PI2;
                var value = (float)(amplitude * Math.Sin((sample * f) / sampleRate));
                sample++;

                samples[index + offset] = value;
                if (channels == 2)
                {
                    samples[index + offset + 1] = value;
                }

                if (sample > sampleRate)
                {
                    sample = 0;
                }

                sweepTotal += Sweep;

                index += channels;
            }
        }

        sampleCountTotal += count;

        var returnValue = 0;
        if (sampleCountTotal < sampleRate)
        {
            returnValue = count;
        }

        Console.WriteLine("sampleCountTotal: {0}", sampleCountTotal);
        return returnValue;
    }
}
