#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;
using KataSoundSynthesizer.Tone;

namespace KataSoundSynthesizer.Oscillators;

class WaveFormOscillator : SynthComponentBase, IOscillator
{
    private readonly float[] waveForm;
    private readonly IOscillator fmModulator;
    private readonly Slew slew;
    private float accu;

    public WaveFormOscillator(float[] waveForm, IOscillator fmModulator, Slew slew)
    {
        if (waveForm == null)
        {
            throw new ArgumentNullException("waveForm");
        }

        if (fmModulator == null)
        {
            throw new ArgumentNullException("fmModulator");
        }

        if (slew == null)
        {
            throw new ArgumentNullException("slew");
        }

        this.waveForm = waveForm;
        this.fmModulator = fmModulator;
        this.slew = slew;
        Multiplier = OctaveMultiplier.x1;
    }

    public WaveFormOscillator(WaveFormOscillator osc)
    {
        waveForm = osc.waveForm;
        fmModulator = osc.fmModulator;
        slew = osc.slew;

        SampleRate = osc.SampleRate;
        WaveForm = osc.WaveForm;
        Amplitude = osc.Amplitude;
        Frequency = osc.Frequency;
        Phase = osc.Phase;
        DutyCycle = osc.DutyCycle;
        CutOff = osc.CutOff;
        Detune = osc.Detune;
        FmLevel = osc.FmLevel;
        Multiplier = osc.Multiplier;
    }

    public override int SampleRate { get; set; }

    public override ISynthComponent MakeInstanceCopy()
    {
        return new WaveFormOscillator(this);
    }

    public WaveFormEnum WaveForm { get; set; }
    public float Amplitude { get; set; }
    public float Frequency { get; set; }
    public float Phase { get; set; }
    public float DutyCycle { get; set; }
    public float CutOff { get; set; }
    public float Detune { get; set; }
    public float FmLevel { get; set; }
    public OctaveMultiplier Multiplier { get; set; }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);

        fmModulator.RenderSamples(offset, count);
        slew.RenderSamples(offset, count);
        var fmBuffer = fmModulator.GetMonoBuffer();
        var slewBuffer = slew.GetMonoBuffer();
        var modulatorNote = 0f;
        var frequency = 0f;

        var sampleCount = offset + count;
        for (var i = offset; i < sampleCount; ++i)
        {
            modulatorNote = 12 * fmBuffer[i] * FmLevel + Frequency * (int)Multiplier;
            modulatorNote += 12 * slewBuffer[i];

            if (modulatorNote < 0)
            {
                modulatorNote = 0;
            }
            else if (modulatorNote >= Scale.MaxToneIndex)
            {
                modulatorNote = Scale.MaxToneIndex - 1;
            }

            frequency = (float)(
                PowerOfTwoTable.GetPower((modulatorNote - Scale.A440ToneIndex) / 12) * Scale.A440
            );
            accu += frequency * waveForm.Length / SampleRate;

            if (accu >= waveForm.Length)
            {
                accu -= waveForm.Length;
            }

            monoBuffer[i] = waveForm[(int)accu] * Amplitude * Phase;
        }
    }
}
