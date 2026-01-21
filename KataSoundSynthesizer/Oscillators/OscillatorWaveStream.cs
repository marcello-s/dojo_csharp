#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer.Oscillators;

class OscillatorWaveStream(WaveFormat waveFormat, ISynthComponent oscillator, int durationInSeconds)
    : WaveStreamBase(waveFormat)
{
    private readonly ISynthComponent synthComponent = oscillator;
    private readonly int durationInSeconds = durationInSeconds;
    private long totalSampleCount;

    protected override int Read(float[]? samples, int offset, int count)
    {
        if (samples == null)
        {
            return 0;
        }

        var sampleRate = WaveFormat.sampleRate;
        var channels = WaveFormat.channels;
        var sampleCount = count / channels;
        var index = 0;

        synthComponent.RenderSamples(offset, sampleCount);
        var buffer = synthComponent.GetMonoBuffer();
        for (var i = 0; i < buffer.Length; ++i)
        {
            samples[index + offset] = buffer[i];
            if (channels == 2)
            {
                samples[index + offset + 1] = buffer[i];
            }

            index += channels;
        }

        totalSampleCount += sampleCount;
        var returnValue = count;
        if (totalSampleCount / sampleRate > durationInSeconds)
        {
            returnValue = 0;
        }

        return returnValue;
    }
}
