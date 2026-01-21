#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer;

interface IWaveStream
{
    int Read(byte[] samples, int offset, int count);
    WaveFormat WaveFormat { get; }
}
