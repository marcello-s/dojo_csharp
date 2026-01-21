#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

class WinMultiMediaApi
{
    [DllImport("winmm.dll")]
    public static extern Int32 mmioStringToFOURCC(
        [MarshalAs(UnmanagedType.LPStr)] string s,
        int flags
    );

    [DllImport("winmm.dll")]
    public static extern Int32 waveOutGetNumDevs();

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutPrepareHeader(
        IntPtr hWaveOut,
        WaveHeader lpWaveOutHdr,
        int uSize
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutUnprepareHeader(
        IntPtr hWaveOut,
        WaveHeader lpWaveOutHr,
        int uSize
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutWrite(IntPtr hWaveOut, WaveHeader lpWaveOutHdr, int uSize);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutOpen(
        out IntPtr hWaveOut,
        IntPtr uDeviceId,
        WaveFormat lpFormat,
        WaveCallback dwCallback,
        IntPtr dwInstance,
        WaveInOutOpenFlags dwFlags
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutOpenWindow(
        out IntPtr hWaveOut,
        IntPtr uDeviceId,
        WaveFormat lpFormat,
        IntPtr callbackWindowHandle,
        IntPtr dwInstance,
        WaveInOutOpenFlags dwFlags
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutReset(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutClose(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutPause(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutRestart(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, out MmTime mmTime, int uSize);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

    [DllImport("winmm.dll")]
    public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

    [DllImport("winmm.dll", CharSet = CharSet.Auto)]
    public static extern MmResult waveOutGetDevCaps(
        IntPtr deviceId,
        out WaveOutCapabilities waveOutCaps,
        int waveOutCapsSize
    );

    [DllImport("winmm.dll")]
    public static extern Int32 waveInGetNumDevs();

    [DllImport("winmm.dll", CharSet = CharSet.Auto)]
    public static extern MmResult waveInGetDevCaps(
        IntPtr deviceId,
        out WaveInCapabilities waveInCaps,
        int waveInCapsSize
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, WaveHeader pwh, int cbwh);

    [DllImport("winmm.dll")]
    public static extern MmResult waveInClose(IntPtr hWaveIn);

    [DllImport("winmm.dll")]
    public static extern MmResult waveInOpen(
        out IntPtr hWaveIn,
        IntPtr uDeviceId,
        WaveFormat lpFormat,
        WaveCallback dwCallback,
        IntPtr dwInstance,
        WaveInOutOpenFlags dwFlags
    );

    [DllImport("winmm.dll", EntryPoint = "waveInOpen")]
    public static extern MmResult waveInOpenWindow(
        out IntPtr hWaveIn,
        IntPtr uDeviceId,
        WaveFormat lpFormat,
        IntPtr callbackWindowHandle,
        IntPtr dwInstance,
        WaveInOutOpenFlags dwFlags
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveInPrepareHeader(
        IntPtr hWaveIn,
        WaveHeader lpWaveHdr,
        int uSize
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveInUnprepareHeader(
        IntPtr hWaveIn,
        WaveHeader lpWaveHdr,
        int uSize
    );

    [DllImport("winmm.dll")]
    public static extern MmResult waveInReset(IntPtr hWaveIn);

    [DllImport("winmm.dll")]
    public static extern MmResult waveInStart(IntPtr hWaveIn);

    [DllImport("winmm.dll")]
    public static extern MmResult waveInStop(IntPtr hWaveIn);
}

[Flags]
public enum WaveInOutOpenFlags
{
    CallbackNull = 0x00,
    CallbackWindow = 0x10000,
    CallbackThread = 0x20000,
    CallbackFunction = 0x30000,
    CallbackEvent = 0x50000,
}

public enum WaveMessage
{
    WaveInOpen = 0x3be,
    WaveInClose = 0x3bf,
    WaveInData = 0x3c0,
    WaveOutClose = 0x3bc,
    WaveOutDone = 0x3bd,
    WaveOutOpen = 0x3bb,
}

public enum MmResult
{
    NoError = 0,
    UnspecifiedError = 1,
    BadDeviceId = 2,
    NotEnabled = 3,
    AlreadyAllocated = 4,
    InvalidHandle = 5,
    NoDriver = 6,
    MemoryAllocationError = 7,
    NotSupported = 8,
    BadErrorNumber = 9,
    InvalidFlag = 10,
    InvalidParameter = 11,
    HandleBusy = 12,
    InvalidAlias = 13,
    BadRegistryDatabase = 14,
    RegistryKeyNotFound = 15,
    RegistryReadError = 16,
    RegistryWriteError = 17,
    RegistryDeleteError = 18,
    RegistryValueNotFound = 19,
    NoDriverCallback = 20,
    MoreData = 21,

    WaveBadFormat = 32,
    WaveStillPlaying = 33,
    WaveHeaderUnprepared = 34,
    WaveSync = 35,

    AcmNotPossible = 512,
    AcmBusy = 513,
    AcmHeaderUnprepared = 514,
    AcmCancelled = 515,

    MixerInvalidLine = 1024,
    MixerInvalidControl = 1025,
    MixerInvalidValue = 1026,
}

[StructLayout(LayoutKind.Explicit)]
public struct MmTime
{
    public const int TimeMs = 0x01;
    public const int TimeSamples = 0x02;
    public const int TimeBytes = 0x04;

    [FieldOffset(0)]
    public UInt32 wType;

    [FieldOffset(4)]
    public UInt32 ms;

    [FieldOffset(4)]
    public UInt32 sample;

    [FieldOffset(4)]
    public UInt32 cb;

    [FieldOffset(4)]
    public UInt32 ticks;

    [FieldOffset(4)]
    public byte smpteHour;

    [FieldOffset(5)]
    public byte smpteMin;

    [FieldOffset(6)]
    public byte smpteSec;

    [FieldOffset(7)]
    public byte smpteFrame;

    [FieldOffset(8)]
    public byte smpteFps;

    [FieldOffset(9)]
    public byte smpteDummy;

    [FieldOffset(10)]
    public byte smptePad0;

    [FieldOffset(11)]
    public byte smptePad1;

    [FieldOffset(4)]
    public UInt32 midiSongPtrPos;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct WaveOutCapabilities
{
    private const int MaxProductNameLength = 32;

    public short manufacturerId;
    public short productId;
    public int driverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
    public string productName;
    public SupportedWaveFormat supportedWaveFormat;
    public short channels;
    public short reserved;
    public WaveOutSupport waveOutSupport;
    public Guid manufacturerGuid;
    public Guid productGuid;
    public Guid nameGuid;

    public bool SupportsPlaybackControlRate
    {
        get
        {
            return (waveOutSupport & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate;
        }
    }

    public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
    {
        return (supportedWaveFormat & waveFormat) == waveFormat;
    }
}

[Flags]
public enum SupportedWaveFormat
{
    // 1 = 11.025 kHz, 2 = 22.05 kHz, 4 = 44.1 kHz, 48 = 48 kHz, 96 = 96 kHz
    // M = Mono, S = Stereo
    // 08 = 8bit, 16 = 16bit
    WaveFormat1M08 = 0x00000001,
    WaveFormat1S08 = 0x00000002,
    WaveFormat1M16 = 0x00000004,
    WaveFormat1S16 = 0x00000008,
    WaveFormat2M08 = 0x00000010,
    WaveFormat2S08 = 0x00000020,
    WaveFormat2M16 = 0x00000040,
    WaveFormat2S16 = 0x00000080,
    WaveFormat4M08 = 0x00000100,
    WaveFormat4S08 = 0x00000200,
    WaveFormat4M16 = 0x00000400,
    WaveFormat4S16 = 0x00000800,
    WaveFormat48M08 = 0x00001000,
    WaveFormat48S08 = 0x00002000,
    WaveFormat48M16 = 0x00004000,
    WaveFormat48S16 = 0x00008000,
    WaveFormat96M08 = 0x00010000,
    WaveFormat96S08 = 0x00020000,
    WaveFormat96M16 = 0x00040000,
    WaveFormat96S16 = 0x00080000,
}

[Flags]
public enum WaveOutSupport
{
    Pitch = 0x001,
    PlaybackRate = 0x0002,
    Volume = 0x0004,
    LRVolume = 0x0008,
    Sync = 0x0010,
    SampleAccurate = 0x0020,
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct WaveInCapabilities
{
    private const int MaxProductNameLenght = 32;

    public short manufacturerId;
    public short productId;
    public int driverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLenght)]
    public string productName;
    public SupportedWaveFormat supportedFormats;
    public short channels;
    public short reserved;
    public Guid manufactererGuid;
    public Guid productGuid;
    public Guid nameGuid;

    public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
    {
        return (supportedFormats & waveFormat) == waveFormat;
    }
}

public delegate void WaveCallback(
    IntPtr hWaveOut,
    WaveMessage message,
    IntPtr dwInstance,
    WaveHeader? wavhdr,
    IntPtr dwReserved
);
