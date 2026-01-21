#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Midi.Device;

public struct MidiParams(IntPtr param1, IntPtr param2)
{
    public readonly IntPtr Param1 = param1;
    public readonly IntPtr Param2 = param2;
}
