#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer;

[Flags]
public enum WaveHeaderFlags
{
    Done = 0x01,
    Prepared = 0x02,
    BeginLoop = 0x04,
    EndLoop = 0x08,
    InQueue = 0x10,
}
