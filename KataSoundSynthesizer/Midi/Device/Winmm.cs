#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer.Midi.Device;

public class Winmm
{
    [DllImport("winmm.dll")]
    public static extern int midiConnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

    [DllImport("winmm.dll")]
    public static extern int midiDisconnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

    public delegate void ProcessMidiIn(
        IntPtr handle,
        int msg,
        IntPtr instance,
        IntPtr param1,
        IntPtr param2
    );

    [DllImport("winmm.dll")]
    public static extern int midiInOpen(
        out IntPtr handle,
        int deviceId,
        ProcessMidiIn processing,
        IntPtr instance,
        int flags
    );

    [DllImport("winmm.dll")]
    public static extern int midiInClose(IntPtr handle);

    [DllImport("winmm.dll")]
    public static extern int midiInStart(IntPtr handle);

    [DllImport("winmm.dll")]
    public static extern int midiInStop(IntPtr handle);

    [DllImport("winmm.dll")]
    public static extern int midiInReset(IntPtr handle);

    [DllImport("winmm.dll")]
    public static extern int midiInPrepareHeader(IntPtr handle, IntPtr header, int headerSize);

    [DllImport("winmm.dll")]
    public static extern int midiInUprepareHeader(IntPtr handle, IntPtr header, int headerSize);

    [DllImport("winmm.dll")]
    public static extern int midiInAddBuffer(IntPtr handle, IntPtr header, int headerSize);

    [DllImport("winmm.dll")]
    public static extern int midiInGetDevCaps(IntPtr deviceId, ref MidiInCaps caps, int capsSize);

    [DllImport("winmm.dll")]
    public static extern int midiInGetNumDevs();

    public const int CallbackFunction = 0x30000;

    public const int MidiIoStatus = 0x00000020;
    public const int MimOpen = 0x3C1;
    public const int MimClose = 0x3C2;
    public const int MimData = 0x3C3;
    public const int MimLongData = 0x3C4;
    public const int MimError = 0x3C5;
    public const int MimLongError = 0x3C6;
    public const int MimMoreData = 0x3CC;
    public const int MhdrDone = 0x00000001;
}
