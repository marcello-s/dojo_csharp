#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Effect;
using KataSoundSynthesizer.Oscillators;
using KataSoundSynthesizer.Tone;

namespace KataSoundSynthesizer.SynthComponent;

class Voice : IVoice
{
    private readonly IFilter filter;
    private readonly IOscillator voiceOscillator;
    private readonly IAdsrEnvelope adsr;
    private readonly IOscillator fmModulator;
    private float frequency;
    private readonly float amplitudeGain;
    private readonly float frequencyGain;
    private readonly float filterGain;
    private readonly IEffectComponent? effect;
    private readonly Scale scale;
    private readonly bool autoPanning;
    private float panning = 0.5f;

    //private float[] _samples;
    //private readonly float[] _samplesNoEffect = new float[2];
    //private readonly float[] _silence = new float[2];
    private bool isTriggered;
    private readonly MonoToStereoConverter converter;

    public float Panning
    {
        get { return panning; }
        set
        {
            if (value < 0.0f || value > 1.0f)
            {
                throw new ArgumentOutOfRangeException("Panning", "allowed range: 0.0 - 1.0");
            }

            panning = value;
        }
    }

    public Voice(
        IFilter filter,
        IOscillator voiceOscillator,
        IAdsrEnvelope adsr,
        IOscillator fmModulator,
        float amplitudeGain,
        float frequencyGain,
        float filterGain,
        IEffectComponent? effect,
        bool autoPanning
    )
    {
        this.filter = filter;
        this.voiceOscillator = voiceOscillator;
        this.adsr = adsr;
        this.fmModulator = fmModulator;
        this.amplitudeGain = amplitudeGain;
        this.frequencyGain = frequencyGain;
        this.filterGain = filterGain;
        this.effect = effect;
        this.autoPanning = autoPanning;
        frequency = this.voiceOscillator.Frequency;
        scale = new Scale();
        converter = new MonoToStereoConverter(this.filter);
    }

    private Voice(Voice voice)
    {
        voiceOscillator = (IOscillator)voice.voiceOscillator.MakeInstanceCopy();
        adsr = (IAdsrEnvelope)voice.adsr.MakeInstanceCopy();
        fmModulator = (IOscillator)voice.fmModulator.MakeInstanceCopy();

        var compOsc = (CompositeOscillator)voiceOscillator;
        filter = new StateVariableFilter(
            compOsc.Components.ElementAt(0),
            compOsc.Components.ElementAt(1),
            adsr,
            fmModulator
        )
        {
            Resonance = voice.filter.Resonance,
            CutOffFrequency = voice.filter.CutOffFrequency,
            Filter = voice.filter.Filter,
            Drive = voice.filter.Drive,
            FilterGain = voice.filter.FilterGain,
            Amplitude = voice.filter.Amplitude,
        };

        amplitudeGain = voice.amplitudeGain;
        frequencyGain = voice.frequencyGain;
        filterGain = voice.filterGain;

        if (voice.effect != null)
        {
            effect = voice.effect.MakeInstanceCopy();
        }

        frequency = voice.frequency;
        scale = new Scale();
        panning = voice.panning;
        autoPanning = voice.autoPanning;
        converter = new MonoToStereoConverter(filter);
    }

    public IVoice MakeInstanceCopy()
    {
        return new Voice(this);
    }

    //public float[] GetSamples(long t)
    //{
    //    if (!_isTriggered)
    //    {
    //        return _silence;
    //    }

    //    var amplitude = _amplitudeGain;

    //    if (_adsr != null)
    //    {
    //        amplitude = Clamp(_amplitudeGain*_adsr.GetSample(t));
    //        _filter.Frequency = ClampNegative(_filter.Frequency + _filterGain * _adsr.GetSample(t));
    //    }

    //    if (_fmModulator != null)
    //    {
    //        _voiceOscillator.Frequency = ClampNegative(_frequency + _frequencyGain*_fmModulator.GetSample(t));
    //    }

    //    var sample = amplitude*_filter.GetSample(t);

    //    if (_effect == null)
    //    {
    //        _samplesNoEffect[0] = sample;
    //        _samplesNoEffect[1] = sample;
    //        return _samplesNoEffect;
    //    }

    //    _samples = _effect.GetSamples(sample, sample);

    //    _samples[0] = _samples[0]*(1.0f - Panning);
    //    _samples[1] = _samples[1]*Panning;

    //    return _samples;
    //}

    public void RenderSamples(int offset, int count)
    {
        if (!isTriggered)
        {
            return;
        }

        voiceOscillator.Frequency = ClampNegative(frequency);

        filter.RenderSamples(offset, count);
        converter.RenderSamples(offset, count);

        if (effect != null)
        {
            effect.Apply(converter.GetStereoBuffer());
        }
    }

    public float[,] GetStereoBuffer()
    {
        return converter.GetStereoBuffer();
    }

    private static float Clamp(float value)
    {
        return Math.Max(Math.Min(value, 1.0f), 0.0f);
    }

    private static float ClampNegative(float value)
    {
        return Math.Max(value, 0.0f);
    }

    public void TriggerKey(TrackedKey key)
    {
        isTriggered = true;

        if (adsr == null)
        {
            return;
        }

        frequency = (float)scale.Tones[key.NoteNumber];
        adsr.Trigger(key.NoteNumber, key.Velocity);
        filter.TriggerKey(key.NoteNumber);

        if (autoPanning)
        {
            Panning = GetAutoPanningValue(key.NoteNumber, Scale.MaxToneIndex);
        }
    }

    public void ReleaseKey(TrackedKey key)
    {
        if (adsr == null)
        {
            return;
        }

        adsr.Release(key.NoteNumber);
    }

    private static float GetAutoPanningValue(int noteNumber, int maxNoteNumber)
    {
        return 1.0f / ((float)maxNoteNumber / noteNumber);
    }
}
