#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NAudio.Wave;

namespace KataSoundSynthesizer;

class NAudioSinewaveProvider32 : WaveProvider32
{
    private int sample;
    private int sampleCountTotal;
    private const float PI2 = (float)(2 * Math.PI);
    private float frequency_PI2;

    public NAudioSinewaveProvider32(float frequency, float amplitude)
    {
        Frequency = frequency;
        Amplitude = amplitude;

        frequency_PI2 = PI2 * Frequency;
    }

    public float Frequency { get; private set; }
    public float Amplitude { get; private set; }

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        var sampleRate = WaveFormat.SampleRate;
        var channels = WaveFormat.Channels;
        var index = 0;

        for (var n = 0; n < sampleCount / channels; ++n)
        {
            var value = (float)(Amplitude * Math.Sin((sample * frequency_PI2) / sampleRate));
            sample++;

            buffer[index + offset] = value;

            if (channels == 2)
            {
                buffer[index + offset + 1] = value;
            }

            if (sample >= sampleRate)
            {
                sample = 0;
            }

            index += channels;
        }

        sampleCountTotal += sampleCount;

        var returnValue = 0;
        if (sampleCountTotal < sampleRate)
        {
            returnValue = sampleCount;
        }

        Console.WriteLine("sampleCountTotal: {0}", sampleCountTotal);
        return returnValue;
    }
}
