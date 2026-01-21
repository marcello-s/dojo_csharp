#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

[StructLayout(LayoutKind.Sequential)]
public class WaveHeader
{
    public IntPtr dataBuffer;
    public int bufferLength;
    public int bytesRecorded;
    public IntPtr userData;
    public WaveHeaderFlags flags;
    public int loops;
    public IntPtr next;
    public IntPtr reserved;
}
