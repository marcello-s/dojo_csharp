#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Oscillators;
using KataSoundSynthesizer.Tone;

namespace KataSoundSynthesizer.SynthComponent;

class StateVariableFilter : SynthComponentBase, IFilter
{
    private const int FmScaler = 10;
    private const int NotesPerOctave = 12;
    private const float FrequencyMax = 12543.85366f;
    private const float DriveOffset = 0.5f;
    private const float DriveScale = 5.0f;
    private const float OutputMax = 0.66666f;
    private const int MiddleCNoteIndex = 40;
    private readonly ISynthComponent source1;
    private readonly ISynthComponent source2;
    private readonly ISynthComponent? am;
    private readonly ISynthComponent? lfo;
    private float output1;
    private float output2;
    private float frequency;
    private float cutOffFrequency;
    private int trackedNote;

    public override int SampleRate { get; set; }
    public float Resonance { get; set; }
    public float Drive { get; set; }
    public FilterType Filter { get; set; }
    public float FilterGain { get; set; }
    public float Amplitude { get; set; }

    public float CutOffFrequency
    {
        get { return cutOffFrequency; }
        set { cutOffFrequency = value > FrequencyMax ? FrequencyMax : value; }
    }

    public StateVariableFilter(ISynthComponent source1, ISynthComponent source2)
    {
        this.source1 = source1;
        this.source2 = source2;

        Resonance = 0.0f;
        CutOffFrequency = (float)Scale.A440;
        Filter = FilterType.LowPass;
        Drive = 1.0f;
        trackedNote = 70;
    }

    public StateVariableFilter(
        ISynthComponent source1,
        ISynthComponent source2,
        ISynthComponent am,
        ISynthComponent lfo
    )
        : this(source1, source2)
    {
        this.am = am;
        this.lfo = lfo;
    }

    private StateVariableFilter(StateVariableFilter filter)
    {
        source1 = filter.source1.MakeInstanceCopy();
        source2 = filter.source2.MakeInstanceCopy();
        if (filter.am != null)
        {
            am = filter.am.MakeInstanceCopy();
        }

        if (filter.lfo != null)
        {
            lfo = filter.lfo.MakeInstanceCopy();
        }

        FilterGain = filter.FilterGain;
        Amplitude = filter.Amplitude;

        Resonance = filter.Resonance;
        CutOffFrequency = filter.CutOffFrequency;
        Filter = filter.Filter;
        Drive = filter.Drive;
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        return new StateVariableFilter(this);
    }

    public override void RenderSamples(int offset, int count)
    {
        if (am == null || lfo == null)
        {
            throw new InvalidOperationException(
                "AM or LFO component is not set for StateVariableFilter"
            );
        }

        InitialiseMonoBuffer(count);
        Array.Clear(monoBuffer, 0, monoBuffer.Length);

        var output = 0.0f;

        am.RenderSamples(offset, count);
        var amBuffer = am.GetMonoBuffer();
        lfo.RenderSamples(offset, count);
        var lfoBuffer = lfo.GetMonoBuffer();

        var f = 1f;
        var fb = 1f;
        var fm = 1f;
        var modNote = 1f;

        source1.RenderSamples(offset, count);
        var source1Buffer = source1.GetMonoBuffer();
        source2.RenderSamples(offset, count);
        var source2Buffer = source2.GetMonoBuffer();
        var input = 0f;

        var sampleCount = offset + count;
        for (var i = offset; i < sampleCount; ++i)
        {
            fm = FilterGain * (amBuffer[i] + lfoBuffer[i]);
            modNote = trackedNote + NotesPerOctave * FmScaler * fm;

            if (modNote < 0)
            {
                modNote = 0f;
            }
            else if (modNote > Scale.MaxToneIndex - 1)
            {
                modNote = Scale.MaxToneIndex - 1;
            }

            frequency = (float)(
                PowerOfTwoTable.GetPower((modNote - Scale.A440ToneIndex) / NotesPerOctave)
                * Scale.A440
            );
            frequency = ClampMinMax(frequency, 0f, FrequencyMax);
            //_frequency = ClampMinMax(_frequency + FilterGain * amBuffer[i], 0.0f, FrequencyMax);
            f = frequency / FrequencyMax;
            fb = Resonance + Resonance / (1.0f - f);

            input =
                (source1Buffer[i] + source2Buffer[i]) / 2.0f * (Drive * DriveScale + DriveOffset);
            output1 += f * (input - output1 + fb * (output1 - output2));
            output2 += f * (output1 - output2);

            switch (Filter)
            {
                case FilterType.LowPass:
                    output = output2;
                    break;

                case FilterType.HighPass:
                    output = input - output2;
                    break;

                case FilterType.BandPass:
                    output = output1 - output2;
                    break;

                case FilterType.Notch:
                    output = input - (output1 - output2);
                    break;

                default:
                    break;
            }

            if (output >= 1.0f)
            {
                output = OutputMax;
            }
            else if (output <= -1.0f)
            {
                output = -OutputMax;
            }
            else
            {
                output = output - output * output * output / 3.0f;
            }

            monoBuffer[i] += output * Clamp(Amplitude * amBuffer[i]);
        }
    }

    public void TriggerKey(int noteNumber)
    {
        var frequencyNote = Scale.MaxToneIndex * cutOffFrequency;
        trackedNote = (int)Math.Round(frequencyNote + (noteNumber - MiddleCNoteIndex));

        if (trackedNote < 0)
        {
            trackedNote = 0;
        }
        else if (trackedNote >= Scale.MaxToneIndex)
        {
            trackedNote = Scale.MaxToneIndex;
        }
    }

    private static float Clamp(float value)
    {
        return Math.Max(Math.Min(value, 1.0f), 0.0f);
    }

    private static float ClampMinMax(float value, float min, float max)
    {
        return Math.Max(Math.Min(value, max), min);
    }
}

public enum FilterType
{
    LowPass,
    HighPass,
    BandPass,
    Notch,
}
