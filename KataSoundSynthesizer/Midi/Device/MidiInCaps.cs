#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer.Midi.Device;

[StructLayout(LayoutKind.Sequential)]
public struct MidiInCaps
{
    public short mid;
    public short pid;
    public int driverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string name;

    public int support;
}
