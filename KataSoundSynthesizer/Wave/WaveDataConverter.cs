#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Wave;

static class WaveDataConverter
{
    public static float[] ConvertToFloatBuffer(WaveData waveData)
    {
        var bufferSize =
            waveData.Data == null
                ? 0
                : waveData.Data.Length / ((waveData.BitsPerSample / 8) * waveData.Channels);
        var buffer = new float[bufferSize];

        var j = 0;
        var raw = new byte[2];
        if (waveData.Data != null)
        {
            for (var i = 0; i < bufferSize; ++i)
            {
                raw[0] = waveData.Data[j];
                raw[1] = waveData.Data[j + 1];

                var sample = Endianess.ConvertUintLittleToBig16(raw);
                sample = (ushort)(sample - ushort.MaxValue / 2);
                buffer[i] = ((float)sample / ushort.MaxValue);

                j += 2;
            }
        }

        return buffer;
    }

    public static float[] ReverseFloatBuffer(float[] buffer)
    {
        var reversed = new float[buffer.Length];

        for (var i = 0; i < buffer.Length; ++i)
        {
            reversed[reversed.Length - (i + 1)] = buffer[i];
        }

        return reversed;
    }
}
