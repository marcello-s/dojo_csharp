#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class AdsrEnvelope : SynthComponentBase, IAdsrEnvelope
{
    private enum State
    {
        Attack,
        Decay,
        Release,
        Completed,
    }

    private const int TimeScaler = 3;
    private const float Ceiling = 0.63212f; // -3dB
    private State state = State.Completed;
    private float a0,
        b1;
    private float output;
    private float amplitude;

    public override int SampleRate { get; set; }
    public float AttackTime { get; set; }
    public float DecayTime { get; set; }
    public float SustainLevel { get; set; }
    public float ReleaseTime { get; set; }
    public float VelocitySensitivity { get; set; }
    public TriggerModeEnum TriggerMode { get; set; }
    public float SlewTime { get; set; }

    public AdsrEnvelope()
    {
        DecayTime = 0.25f;
        ReleaseTime = 0.25f;
        TriggerMode = TriggerModeEnum.Retrigger;
        output = 1.0f;
        amplitude = 1.0f;
    }

    private AdsrEnvelope(IAdsrEnvelope adsrEnvelope)
    {
        SampleRate = adsrEnvelope.SampleRate;
        AttackTime = adsrEnvelope.AttackTime;
        DecayTime = adsrEnvelope.DecayTime;
        SustainLevel = adsrEnvelope.SustainLevel;
        ReleaseTime = adsrEnvelope.ReleaseTime;
        VelocitySensitivity = adsrEnvelope.VelocitySensitivity;
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        return new AdsrEnvelope(this);
    }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);

        var sampleCount = offset + count;
        for (var i = offset; i < sampleCount; ++i)
        {
            switch (state)
            {
                case State.Attack:
                    output = a0 + b1 * output;

                    if (output >= Ceiling + 1.0f)
                    {
                        var d = DecayTime * TimeScaler * SampleRate + 1.0f;
                        var x = (float)Math.Exp(-1.0 / d);

                        a0 = 1 - x;
                        b1 = x;

                        output = Ceiling + 1;

                        state = State.Decay;
                    }
                    break;

                case State.Decay:
                    output = a0 * SustainLevel + b1 * output;
                    break;

                case State.Release:
                    output = a0 + b1 * output;

                    if (output < 0.0f)
                    {
                        output = 1.0f;
                        state = State.Completed;
                    }
                    break;

                case State.Completed:
                    break;

                default:
                    break;
            }

            monoBuffer[i] = (output - 1.0f) / Ceiling * amplitude;
        }
    }

    public void Trigger(int noteNumber, float velocity)
    {
        if (
            TriggerMode == TriggerModeEnum.Legato
            && (state == State.Attack || state == State.Decay)
        )
        {
            return;
        }

        //var d = AttackTime*TimeScaler*_sampleRate + 1.0f;
        var x = (float)Math.Exp(-1.0 / (AttackTime * TimeScaler * SampleRate));

        a0 = (1.0f - x) * 2.0f;
        b1 = x;

        amplitude = (float)
            Math.Pow((1.0f - VelocitySensitivity) + velocity * VelocitySensitivity, 2);

        state = State.Attack;
    }

    public void Release(int noteNumber)
    {
        if (state == State.Release || state == State.Completed)
        {
            return;
        }

        var d = ReleaseTime * TimeScaler * SampleRate + 1.0f;
        var x = (float)Math.Exp(-1.0 / d);

        a0 = (1.0f - x) * 0.9f;
        b1 = x;

        state = State.Release;
    }
}

public enum TriggerModeEnum
{
    Retrigger,
    Legato,
}
