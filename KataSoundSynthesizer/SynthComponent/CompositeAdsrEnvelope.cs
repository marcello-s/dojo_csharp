#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class CompositeAdsrEnvelope : SynthComponentBase, IAdsrEnvelope
{
    private readonly IList<IAdsrEnvelope> adsrEnvelopes = new List<IAdsrEnvelope>();

    public void Trigger(int noteNumber, float velocity)
    {
        foreach (var adsrEnvelope in adsrEnvelopes)
        {
            adsrEnvelope.Trigger(noteNumber, velocity);
        }
    }

    public void Release(int noteNumber)
    {
        foreach (var adsrEnvelope in adsrEnvelopes)
        {
            adsrEnvelope.Release(noteNumber);
        }
    }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);
        Array.Clear(monoBuffer, 0, monoBuffer.Length);

        foreach (var adsrEnvelope in adsrEnvelopes)
        {
            adsrEnvelope.RenderSamples(offset, count);
            var buffer = adsrEnvelope.GetMonoBuffer();
            for (var i = 0; i < buffer.Length; ++i)
            {
                monoBuffer[i] += buffer[i];
            }
        }
    }

    public override int SampleRate
    {
        get { return adsrEnvelopes.First().SampleRate; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.SampleRate = value;
            }
        }
    }

    public float AttackTime
    {
        get { return adsrEnvelopes.First().AttackTime; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.AttackTime = value;
            }
        }
    }

    public float DecayTime
    {
        get { return adsrEnvelopes.First().DecayTime; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.AttackTime = value;
            }
        }
    }

    public float SustainLevel
    {
        get { return adsrEnvelopes.First().SustainLevel; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.SustainLevel = value;
            }
        }
    }

    public float ReleaseTime
    {
        get { return adsrEnvelopes.First().ReleaseTime; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.ReleaseTime = value;
            }
        }
    }

    public float VelocitySensitivity
    {
        get { return adsrEnvelopes.First().VelocitySensitivity; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.VelocitySensitivity = value;
            }
        }
    }

    public TriggerModeEnum TriggerMode
    {
        get { return adsrEnvelopes.First().TriggerMode; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.TriggerMode = value;
            }
        }
    }

    public float SlewTime
    {
        get { return adsrEnvelopes.First().SlewTime; }
        set
        {
            foreach (var adsrEnvelope in adsrEnvelopes)
            {
                adsrEnvelope.SlewTime = value;
            }
        }
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        var cae = new CompositeAdsrEnvelope();

        foreach (var env in adsrEnvelopes)
        {
            cae.AddAdsrEnvelope((IAdsrEnvelope)env.MakeInstanceCopy());
        }

        return cae;
    }

    public void AddAdsrEnvelope(IAdsrEnvelope adsrEnvelope)
    {
        if (adsrEnvelopes.Contains(adsrEnvelope))
        {
            return;
        }

        adsrEnvelopes.Add(adsrEnvelope);
    }
}
