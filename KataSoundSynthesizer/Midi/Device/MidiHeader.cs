#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer.Midi.Device;

[StructLayout(LayoutKind.Sequential)]
public struct MidiHeader
{
    public IntPtr data;
    public int bufferLength;
    public int bytesRecorded;
    public int user;
    public int flags;
    public IntPtr next;
    public int reserved;
    public int offset;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public int[] reservedArray;
}
