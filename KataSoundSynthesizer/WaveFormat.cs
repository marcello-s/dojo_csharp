#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
class WaveFormat
{
    public WaveFormatEncoding waveFormatTag;
    public short channels;
    public int sampleRate;
    public int averageBytesPerSecond;
    public short blockAlign;
    public short bitsPerSample;
    public short extraSize;

    private WaveFormat() { }

    public static WaveFormat MakeIeeeFloatWaveFormat(int sampleRate, short channels)
    {
        var wf = new WaveFormat
        {
            waveFormatTag = WaveFormatEncoding.IeeeFloat,
            channels = channels,
            sampleRate = sampleRate,
            blockAlign = (short)(4 * channels),
            bitsPerSample = 32,
            extraSize = 0,
        };
        wf.averageBytesPerSecond = wf.sampleRate * wf.blockAlign;

        return wf;
    }

    public int ConvertLatencyToByteSize(int milliseconds)
    {
        var bytes = (int)(averageBytesPerSecond / 1000.0) * milliseconds;
        if ((bytes % blockAlign) != 0)
        {
            bytes = bytes + blockAlign - (bytes % blockAlign);
        }

        return bytes;
    }
}

enum WaveFormatEncoding : ushort
{
    Unknown = 0x00,
    Pcm = 0x01,
    Adpcm = 0x02,
    IeeeFloat = 0x03,
}
