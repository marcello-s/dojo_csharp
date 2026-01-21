#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Oscillators;

static class PowerOfTwoTable
{
    private const int TableSize = 4096;
    private static readonly float[] Table = new float[TableSize];

    static PowerOfTwoTable()
    {
        const float increment = 1.0f / TableSize;
        var accumulator = 0.0f;

        for (var i = 0; i < TableSize; ++i)
        {
            Table[i] = (float)Math.Pow(2.0, accumulator);
            accumulator += increment;
        }
    }

    public static float GetPower(float exponent)
    {
        float result;

        if (exponent >= 0.0f)
        {
            var whole = (int)exponent;
            var fractional = exponent - whole;
            var index = (int)(TableSize * fractional);
            index = Math.Max(0, Math.Min(index, TableSize - 1));
            result = Table[index] * (1 << whole);
        }
        else
        {
            var whole = (int)-exponent;
            var fractional = -exponent - whole;
            var index = (int)(TableSize * fractional);
            index = Math.Max(0, Math.Min(index, TableSize - 1));
            result = 1.0f / (Table[index] * (1 << whole));
        }

        return result;
    }
}
