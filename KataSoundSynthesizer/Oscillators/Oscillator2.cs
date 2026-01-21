#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer.Oscillators;

class Oscillator2 : SynthComponentBase, IOscillator
{
    public const float PI2 = (float)(2 * Math.PI);
    private readonly Random random = new Random();
    private readonly IOscillator? fmModulator;
    private readonly IOscillator? dutyCycleModulator;
    private WaveFormEnum waveForm;
    private float amplitude = 1f;
    private float sine = 0f;
    private float cosine = 1f;
    private float increment;
    private float accumulator;
    private float frequency = 1f;
    private float cutoff = 1f;
    private float phase = 0f;
    private float dutyCycle = (float)Math.PI;
    private float sample;
    private float outputSample;
    private int sampleRate;
    private Func<float>? waveFormFunc;

    public WaveFormEnum WaveForm
    {
        get { return waveForm; }
        set
        {
            waveForm = value;
            sine = 0;
            cosine = 1;

            switch (waveForm)
            {
                case WaveFormEnum.Sine:
                    waveFormFunc = GetSineSample;
                    break;

                case WaveFormEnum.Square:
                    waveFormFunc = GetSquareSample;
                    break;

                case WaveFormEnum.Noise:
                    waveFormFunc = GetNoiseSample;
                    accumulator = 0;
                    break;

                case WaveFormEnum.Triangle:
                    waveFormFunc = GetTriangleSample;
                    accumulator = 0.25f * PI2;
                    break;

                case WaveFormEnum.Sawtooth:
                    waveFormFunc = GetSawtoothSample;
                    accumulator = (float)Math.PI;
                    break;

                default:
                    break;
            }
        }
    }

    public float Amplitude
    {
        get { return amplitude; }
        set
        {
            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException("Amplitude", "allowed range: 0 - 1");
            }

            amplitude = value;
        }
    }

    public float Frequency
    {
        get { return frequency; }
        set
        {
            if (value < 0f)
            {
                throw new ArgumentOutOfRangeException("Frequency", "must be > 0");
            }

            frequency = value;
            CalculateIncrement();
        }
    }

    public float Phase
    {
        get { return phase; }
        set
        {
            if (value < 0f || value > PI2)
            {
                throw new ArgumentOutOfRangeException("Phase", "allowed range: 0 - PI2");
            }

            phase = value;
        }
    }

    public float DutyCycle
    {
        get { return dutyCycle; }
        set
        {
            if (value < 0f || value > PI2)
            {
                throw new ArgumentOutOfRangeException("DutyCycle", "allowed range: 0 - PI2");
            }

            dutyCycle = value;
        }
    }

    public float CutOff
    {
        get { return cutoff; }
        set
        {
            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException("CutOff", "allowed range: 0 - 1");
            }

            cutoff = value;
        }
    }

    public float Detune { get; set; }
    public float FmLevel { get; set; }

    public OctaveMultiplier Multiplier { get; set; }

    public override int SampleRate
    {
        get { return sampleRate; }
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException("SampleRate", "must be >= 1");
            }

            sampleRate = value;
            CalculateIncrement();
        }
    }

    public Oscillator2()
    {
        WaveForm = WaveFormEnum.Sine;
        Multiplier = OctaveMultiplier.x1;
    }

    public Oscillator2(IOscillator fmModulator, bool useFmModulator)
        : this()
    {
        this.fmModulator = fmModulator;
    }

    public Oscillator2(IOscillator? fmModulator, IOscillator dutyCycleModulator)
        : this()
    {
        this.fmModulator = fmModulator;
        this.dutyCycleModulator = dutyCycleModulator;
    }

    public Oscillator2(IOscillator osc)
    {
        Detune = osc.Detune;
        WaveForm = osc.WaveForm;
        Amplitude = osc.Amplitude;
        Frequency = osc.Frequency;
        Phase = osc.Phase;
        DutyCycle = osc.DutyCycle;
        CutOff = osc.CutOff;
        SampleRate = osc.SampleRate;
        Multiplier = osc.Multiplier;
        fmModulator = ((Oscillator2)osc).fmModulator;
    }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);
        var fmBuffer = new float[count];
        if (fmModulator != null)
        {
            fmModulator.RenderSamples(offset, count);
            fmBuffer = fmModulator.GetMonoBuffer();
        }

        var dutyCycleBuffer = new float[count];
        if (dutyCycleModulator != null)
        {
            dutyCycleModulator.RenderSamples(offset, count);
            dutyCycleBuffer = dutyCycleModulator.GetMonoBuffer();
        }

        var sampleCount = offset + count;
        for (var i = offset; i < sampleCount; ++i)
        {
            Frequency += fmBuffer[i] * FmLevel;
            DutyCycle += dutyCycleBuffer[i];
            if (waveFormFunc != null)
            {
                outputSample = waveFormFunc();
            }
            outputSample *= amplitude;
            monoBuffer[i] = outputSample;
        }
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        return new Oscillator2(this);
    }

    private void CalculateIncrement()
    {
        increment = (PI2 - phase) * (frequency * (int)Multiplier + Detune) / SampleRate;
    }

    private void IncrementPhase()
    {
        sine = sine + increment * cosine;
        cosine = cosine - increment * sine;

        accumulator += increment;
        if (accumulator >= PI2)
        {
            accumulator -= PI2;
        }
    }

    private float GetSineSample()
    {
        sample = sine;
        IncrementPhase();
        return sample;
    }

    private float GetSquareSample()
    {
        if (accumulator < dutyCycle)
        {
            sample = 1;
        }
        else
        {
            sample = -1;
        }

        IncrementPhase();
        return sample;
    }

    private float GetTriangleSample()
    {
        if (accumulator < Math.PI)
        {
            sample = 1 - 4 * accumulator / PI2;
        }
        else
        {
            sample = 1 - 4 * (1 - accumulator / PI2);
        }

        IncrementPhase();
        return sample;
    }

    private float GetSawtoothSample()
    {
        sample = 1 - 2 * accumulator / PI2;
        IncrementPhase();
        return sample;
    }

    private float GetNoiseSample()
    {
        sample = (float)(1 - 2 * random.NextDouble());
        return sample;
    }

    private float Cutoff(float sample)
    {
        return (Math.Abs(sample) > cutoff) ? Math.Sign(sample) * cutoff : sample;
    }
}
