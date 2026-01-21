#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;
using NUnit.Framework;

namespace KataSoundSynthesizer.Oscillators;

[TestFixture]
public class Oscillator2Test
{
    [Test]
    public void GetSample_WhenSineWave_ThenGetSineWave()
    {
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Sine, SampleRate = 40 };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSineWaveAndFrequency_ThenGetSineAndFrequency()
    {
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 2f,
            SampleRate = 40,
        };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSquareWave_ThenGetSquareWave()
    {
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Square, SampleRate = 40 };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSquareWaveDutyCycle_ThenGetSquareWave()
    {
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Square,
            DutyCycle = (float)(Math.PI / 2d),
            SampleRate = 40,
        };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenTriangleWave_ThenGetTriangleWave()
    {
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Triangle, SampleRate = 40 };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenNoiseWave_ThenGetNoiseWave()
    {
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Noise, SampleRate = 40 };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSawtoothWave_ThenGetSawtoothWave()
    {
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Sawtooth, SampleRate = 40 };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSineWaveAndCutoff_ThenGetSineWaveAndCutoff()
    {
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            CutOff = 0.85f,
            SampleRate = 40,
        };

        PrintSampleBuffer(osc);
    }

    [Test]
    public void GetSample_WhenSineAndPhase_ThenGetSineAndWave()
    {
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Phase = (float)(Math.PI / 2f),
            SampleRate = 40,
        };

        PrintSampleBuffer(osc);
    }

    private static void PrintSampleBuffer(ISynthComponent comp, bool print = true)
    {
        comp.RenderSamples(0, comp.SampleRate);
        var buffer = comp.GetMonoBuffer();

        if (!print)
        {
            return;
        }

        for (var i = 0; i < buffer.Length; ++i)
        {
            Console.WriteLine(i + ";" + buffer[i]);
            System.Diagnostics.Debug.WriteLine(i + ";" + buffer[i]);
        }
    }
}
