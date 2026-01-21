#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Wave;

class WaveData(short channels, int sampleRate, short bitsPerSample, byte[]? data)
{
    public short Channels { get; private set; } = channels;
    public int SampleRate { get; private set; } = sampleRate;
    public short BitsPerSample { get; private set; } = bitsPerSample;
    public byte[]? Data { get; private set; } = data;
}
