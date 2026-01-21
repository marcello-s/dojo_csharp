#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Effect;

class Reverb : IEffectComponent
{
    private const int BufferSize = 512;
    private float[] buffer = null!;

    public Reverb()
    {
        InitializeDefaults();
    }

    public Reverb(Reverb reverb)
    {
        InitializeDefaults();
    }

    public void Apply(float[,] input)
    {
        var inputLength = input.Length / 2;
        var accuL = 0f;
        var accuR = 0f;
        var l = 0f;
        var r = 0f;

        for (var i = 0; i < inputLength; ++i)
        {
            accuL = 0f;
            accuR = 0f;
            l = input[0, i];
            r = input[1, i];

            for (var j = 0; j < BufferSize; ++j)
            {
                accuL += buffer[j] * l;
                accuR += buffer[j] * r;
            }

            accuL /= BufferSize;
            accuR /= BufferSize;

            input[0, i] = accuL;
            input[1, i] = accuR;
        }
    }

    public IEffectComponent MakeInstanceCopy()
    {
        return new Reverb(this);
    }

    public void Reset()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        buffer = new float[BufferSize];

        const float Min = 0f;
        const float Max = 1f;

        var step = Math.Abs(Max - Min) / (BufferSize / 440);
        float accu = Min;

        for (var i = 0; i < BufferSize; ++i)
        {
            buffer[i] = accu;

            accu += step;
            if (accu > Max)
            {
                accu = Min;
            }
        }
    }
}
