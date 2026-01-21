#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Tone;

class Scale
{
    public const double A440 = 440d;
    public const int A440ToneIndex = 49;
    public const int MaxToneIndex = 88;
    private readonly double scaleStep = Math.Pow(2.0, 1.0 / 12.0);

    public double[] Tones { get; private set; }

    public Scale()
    {
        Tones = CalculateScale(A440, A440ToneIndex, MaxToneIndex);
    }

    private double[] CalculateScale(double baseToneFrequency, int baseToneIndex, int maxToneIndex)
    {
        var tones = new double[maxToneIndex];

        for (var i = 0; i < maxToneIndex; ++i)
        {
            tones[i] = ToneFrequency(baseToneFrequency, baseToneIndex, i);
        }

        return tones;
    }

    public double ToneFrequency(double baseToneFrequency, int baseToneIndex, int toneIndex)
    {
        return baseToneFrequency * Math.Pow(scaleStep, toneIndex - baseToneIndex);
    }
}
