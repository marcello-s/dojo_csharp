#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer.Oscillators;

class CompositeOscillator : SynthComponentBase, IOscillator
{
    private readonly IList<IOscillator> oscillators = new List<IOscillator>();

    public WaveFormEnum WaveForm
    {
        get { return oscillators[0].WaveForm; }
        set { oscillators[0].WaveForm = value; }
    }

    public override int SampleRate
    {
        get { return oscillators[0].SampleRate; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.SampleRate = value;
            }
        }
    }

    public float Amplitude
    {
        get { return oscillators[0].Amplitude; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.Amplitude = value;
            }
        }
    }

    public float Frequency
    {
        get { return oscillators[0].Frequency; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.Frequency = value;
            }
        }
    }

    public float Phase
    {
        get { return oscillators[0].Phase; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.Phase = value;
            }
        }
    }

    public float DutyCycle
    {
        get { return oscillators[0].DutyCycle; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.DutyCycle = value;
            }
        }
    }

    public float CutOff
    {
        get { return oscillators[0].CutOff; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.CutOff = value;
            }
        }
    }

    public float Detune
    {
        get { return oscillators[0].Detune; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.Detune = value;
            }
        }
    }

    public float FmLevel
    {
        get { return oscillators[0].FmLevel; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.FmLevel = value;
            }
        }
    }

    public OctaveMultiplier Multiplier
    {
        get { return oscillators[0].Multiplier; }
        set
        {
            foreach (var oscillator in oscillators)
            {
                oscillator.Multiplier = value;
            }
        }
    }

    public IEnumerable<IOscillator> Components
    {
        get { return oscillators; }
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        var co = new CompositeOscillator();

        foreach (var osc in oscillators)
        {
            co.AddOscillator((IOscillator)osc.MakeInstanceCopy());
        }

        return co;
    }

    public void AddOscillator(IOscillator oscillator)
    {
        if (oscillators.Contains(oscillator))
        {
            return;
        }

        oscillators.Add(oscillator);
    }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);
        Array.Clear(monoBuffer, 0, monoBuffer.Length);

        foreach (var oscillator in oscillators)
        {
            oscillator.RenderSamples(offset, count);
            var buffer = oscillator.GetMonoBuffer();
            for (var i = 0; i < buffer.Length; ++i)
            {
                monoBuffer[i] += buffer[i];
            }
        }
    }
}
