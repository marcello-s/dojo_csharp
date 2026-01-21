#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.SynthComponent;

[TestFixture]
public class AdsrEnvelopeTest
{
    private const int sampleRate = 40;

    private class TestAdsrEnvelope : AdsrEnvelope
    {
        public void SetBuffer(float[] buffer)
        {
            SetMonoBuffer(buffer);
        }
    }

    [Test]
    public void Trigger_WhenAttack_ThenGetAttack()
    {
        var adsr = new TestAdsrEnvelope() { SampleRate = sampleRate, AttackTime = 0.02f };

        adsr.Trigger(0, 1.0f);

        PrintSampleBuffer(adsr);
    }

    [Test]
    public void Release_WhenRelese_ThenGetRelease()
    {
        var adsr = new TestAdsrEnvelope()
        {
            SampleRate = sampleRate,
            AttackTime = 0.02f,
            DecayTime = 0.15f,
        };

        adsr.Trigger(0, 1.0f);

        PrintSampleBuffer(adsr, 7);
    }

    private static void PrintSampleBuffer(
        TestAdsrEnvelope adsr,
        int releaseTime = -1,
        bool print = true
    )
    {
        var buffer = new float[adsr.SampleRate + releaseTime + 1];
        adsr.SetBuffer(buffer);

        if (releaseTime < 0)
        {
            adsr.RenderSamples(0, adsr.SampleRate);
        }
        else
        {
            adsr.RenderSamples(0, releaseTime);
            adsr.Release(0);
            adsr.RenderSamples(releaseTime, adsr.SampleRate);
        }

        buffer = adsr.GetMonoBuffer();

        if (!print)
        {
            return;
        }

        for (var i = 0; i < buffer.Length; ++i)
        {
            Console.WriteLine(i + ";" + buffer[i]);
        }
    }
}
