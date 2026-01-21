#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;
using System.Security;

namespace KataSoundSynthesizer;

class DirectSoundApi
{
    public static readonly Guid DefaultPlayback = new Guid("DEF00000-9C6D-47ED-AAF1-4DDA8F2B5C03");

    private static List<DirectSoundDeviceInfo> devices = null!;
    private IWaveStream waveStream = null!;
    private int latencyInMilliseconds = 40;
    private Guid deviceGuid = Guid.Empty;
    private WaveFormat? waveFormat;
    private IDirectSound directSound = null!;
    private IDirectSoundBuffer primarySoundBuffer = null!;
    private IDirectSoundBuffer secondarySoundBuffer = null!;
    private int samplesFrameSize;
    private int nextSamplesWriteIndex;
    private int samplesTotalSize;
    private byte[]? samples;
    private EventWaitHandle? frameWaitHandle1;
    private EventWaitHandle? frameWaitHandle2;
    private EventWaitHandle? endWaitHandle;
    private long bytesPlayed;

    public static IEnumerable<DirectSoundDeviceInfo> GetDevices()
    {
        devices = new List<DirectSoundDeviceInfo>();
        DirectSoundEnumerate(new DsEnumCallback(EnumCallback), IntPtr.Zero);
        return devices;
    }

    private static bool EnumCallback(
        IntPtr lpGuid,
        IntPtr lpcstrDescription,
        IntPtr lpcstrModule,
        IntPtr lpContext
    )
    {
        var deviceInfo = new DirectSoundDeviceInfo();

        if (lpGuid == IntPtr.Zero)
        {
            deviceInfo.guid = Guid.Empty;
        }
        else
        {
            var guidBytes = new byte[16];
            Marshal.Copy(lpGuid, guidBytes, 0, 16);
            deviceInfo.guid = new Guid(guidBytes);
        }

        deviceInfo.description = Marshal.PtrToStringAnsi(lpcstrDescription);

        if (lpcstrModule != IntPtr.Zero)
        {
            deviceInfo.moduleName = Marshal.PtrToStringAnsi(lpcstrModule);
        }

        devices.Add(deviceInfo);
        return true;
    }

    public DirectSoundApi(IWaveStream waveStream)
    {
        if (waveStream == null)
        {
            throw new ArgumentNullException("waveStream");
        }

        this.waveStream = waveStream;
    }

    private void Initialize()
    {
        waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(44100, 2);

        if (deviceGuid == Guid.Empty)
        {
            deviceGuid = DefaultPlayback;
        }

        directSound = null!;
        DirectSoundCreate(ref deviceGuid, out directSound, IntPtr.Zero);

        if (directSound == null)
        {
            return;
        }

        // set cooperative level to priority
        directSound.SetCooperativeLevel(GetDesktopWindow(), DirectSoundCooperativeLevel.Priority);

        // primary buffer
        var bufferDesc1 = new BufferDescription
        {
            dwBufferBytes = 0,
            dwFlags = DirectSoundBufferCaps.PrimaryBuffer,
            dwReserved = 0,
            lpWxfFormat = IntPtr.Zero,
            guid = Guid.Empty,
        };
        bufferDesc1.dwSize = Marshal.SizeOf(bufferDesc1);

        // create primary buffer
        object soundBuffer;
        directSound.CreateSoundBuffer(bufferDesc1, out soundBuffer, IntPtr.Zero);
        primarySoundBuffer = (IDirectSoundBuffer)soundBuffer;

        // play & loop on the primary sound buffer
        primarySoundBuffer.Play(0, 0, DirectSoundPlayFlags.Looping);

        // a frame of samples of size of the latency
        samplesFrameSize = MsToBytes(latencyInMilliseconds, waveFormat);

        // secondary buffer
        var handleOnWaveFormat = GCHandle.Alloc(waveFormat, GCHandleType.Pinned);
        var bufferDesc2 = new BufferDescription
        {
            dwBufferBytes = (uint)(2 * samplesFrameSize),
            dwFlags =
                DirectSoundBufferCaps.GetCurrentPosition2
                | DirectSoundBufferCaps.CtrlPositionNotify
                | DirectSoundBufferCaps.GlobalFocus
                | DirectSoundBufferCaps.CtrlVolume
                | DirectSoundBufferCaps.StickyFocus,
            dwReserved = 0,
            lpWxfFormat = handleOnWaveFormat.AddrOfPinnedObject(),
            guid = Guid.Empty,
        };
        bufferDesc2.dwSize = Marshal.SizeOf(bufferDesc2);

        // create secondary buffer
        directSound.CreateSoundBuffer(bufferDesc2, out soundBuffer, IntPtr.Zero);
        secondarySoundBuffer = (IDirectSoundBuffer)soundBuffer;
        handleOnWaveFormat.Free();

        // get effective sound buffer size
        var bufferCaps = new BufferCaps();
        bufferCaps.dwSize = Marshal.SizeOf(bufferCaps);
        secondarySoundBuffer.GetCaps(bufferCaps);

        nextSamplesWriteIndex = 0;
        samplesTotalSize = bufferCaps.dwBufferBytes;
        samples = new byte[samplesTotalSize];

        // create double buffering notification
        // use DirectSoundNotify at position [0, 1/2] and stop position 0xFFFFFFFF
        frameWaitHandle1 = new EventWaitHandle(false, EventResetMode.AutoReset);
        frameWaitHandle2 = new EventWaitHandle(false, EventResetMode.AutoReset);
        endWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        var notifies = new DirectSoundBufferPositionNotify[3];
        notifies[0] = new DirectSoundBufferPositionNotify
        {
            dwOffset = 0,
            hEventNotify = frameWaitHandle1.SafeWaitHandle.DangerousGetHandle(),
        };

        notifies[1] = new DirectSoundBufferPositionNotify
        {
            dwOffset = (uint)samplesFrameSize,
            hEventNotify = frameWaitHandle2.SafeWaitHandle.DangerousGetHandle(),
        };

        notifies[2] = new DirectSoundBufferPositionNotify
        {
            dwOffset = 0xFFFFFFFF,
            hEventNotify = endWaitHandle.SafeWaitHandle.DangerousGetHandle(),
        };

        var notify = (IDirectSoundNotify)soundBuffer;
        notify.SetNotificationPositions(3, notifies);
    }

    private static int MsToBytes(int milliseconds, WaveFormat waveFormat)
    {
        var bytes = milliseconds * waveFormat.averageBytesPerSecond / 1000;
        bytes -= bytes % waveFormat.blockAlign;
        return bytes;
    }

    private static bool IsBufferLost(IDirectSoundBuffer soundBuffer)
    {
        return (soundBuffer.GetStatus() & DirectSoundBufferStatus.BufferLost) != 0;
    }

    private int Feed(int bytesToCopy)
    {
        var bytesRead = bytesToCopy;

        // restore buffer if lost
        if (IsBufferLost(secondarySoundBuffer))
        {
            secondarySoundBuffer.Restore();
        }

        // read from waveStream
        if (samples == null)
        {
            return 0;
        }

        bytesRead = waveStream.Read(samples, 0, bytesToCopy);
        if (bytesRead == 0)
        {
            Array.Clear(samples, 0, samples.Length);
            return 0;
        }

        // lock a portion of the secondary buffer starting from 0 or 1/2
        IntPtr waveBuffer1;
        var numberOfSamples1 = 0;
        IntPtr waveBuffer2;
        var numberOfSamples2 = 0;
        secondarySoundBuffer.Lock(
            nextSamplesWriteIndex,
            (uint)bytesRead,
            out waveBuffer1,
            out numberOfSamples1,
            out waveBuffer2,
            out numberOfSamples2,
            DirectSoundBufferLockFlags.None
        );

        // copy back to secondary buffer
        if (waveBuffer1 != IntPtr.Zero)
        {
            Marshal.Copy(samples, 0, waveBuffer1, numberOfSamples1);
            if (waveBuffer2 != IntPtr.Zero)
            {
                Marshal.Copy(samples, 0, waveBuffer2, numberOfSamples2);
            }
        }

        // unlock secondary buffer
        secondarySoundBuffer.Unlock(waveBuffer2, numberOfSamples1, waveBuffer2, numberOfSamples2);

        return bytesRead;
    }

    private void Stop()
    {
        if (secondarySoundBuffer != null)
        {
            secondarySoundBuffer.Stop();
            secondarySoundBuffer = null!;
        }

        if (primarySoundBuffer != null)
        {
            primarySoundBuffer.Stop();
            primarySoundBuffer = null!;
        }
    }

    public void Play()
    {
        var thread = new Thread(PlaybackLoop)
        {
            Priority = ThreadPriority.Normal,
            IsBackground = true,
        };
        thread.Start();
    }

    public void PlaybackLoop()
    {
        bytesPlayed = 0;
        var playbackHalted = false;
        var firstBufferStarted = false;
        Exception? exception = null;

        try
        {
            Initialize();
            if (frameWaitHandle1 == null || frameWaitHandle2 == null || endWaitHandle == null)
            {
                throw new Exception("Waithandles initialization failed");
            }

            secondarySoundBuffer.SetCurrentPosition(0);
            nextSamplesWriteIndex = 0;
            var result = Feed(samplesTotalSize);

            if (result > 0)
            {
                secondarySoundBuffer.Play(0, 0, DirectSoundPlayFlags.Looping);

                var waitHandles = new WaitHandle[]
                {
                    frameWaitHandle1,
                    frameWaitHandle2,
                    endWaitHandle,
                };
                var continuePlayback = true;
                while (continuePlayback)
                {
                    var indexWaitHandle = WaitHandle.WaitAny(
                        waitHandles,
                        3 * latencyInMilliseconds,
                        false
                    );

                    if (indexWaitHandle != WaitHandle.WaitTimeout)
                    {
                        if (indexWaitHandle == 2)
                        {
                            Stop();
                            playbackHalted = true;
                            continuePlayback = false;
                        }
                        else
                        {
                            if (indexWaitHandle == 0)
                            {
                                if (firstBufferStarted)
                                {
                                    bytesPlayed += samplesFrameSize * 2;
                                }
                            }
                            else
                            {
                                firstBufferStarted = true;
                            }

                            indexWaitHandle = (indexWaitHandle == 0) ? 1 : 0;
                            nextSamplesWriteIndex = indexWaitHandle * samplesFrameSize;

                            if (Feed(samplesFrameSize) == 0)
                            {
                                Stop();
                                playbackHalted = true;
                                continuePlayback = false;
                            }
                        }
                    }
                    else
                    {
                        Stop();
                        playbackHalted = true;
                        continuePlayback = false;
                        throw new Exception("DirectSound buffer timeout");
                    }
                }
            }
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            if (!playbackHalted)
            {
                Stop();
            }

            bytesPlayed = 0;
        }
    }

    [DllImport(
        "dsound.dll",
        EntryPoint = "DirectSoundCreate",
        SetLastError = true,
        CharSet = CharSet.Unicode,
        ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall
    )]
    private static extern void DirectSoundCreate(
        ref Guid guid,
        [Out, MarshalAs(UnmanagedType.Interface)] out IDirectSound directSound,
        IntPtr pUnkOuter
    );

    [DllImport(
        "dsound.dll",
        EntryPoint = "DirectSoundEnumerateA",
        SetLastError = true,
        CharSet = CharSet.Unicode,
        ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall
    )]
    private static extern void DirectSoundEnumerate(
        DsEnumCallback lpDSEnumCallback,
        IntPtr lpContext
    );

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();
}

enum DirectSoundCooperativeLevel : uint
{
    Normal = 0x01,
    Priority = 0x02,
    Exclusive = 0x03,
    WritePrimary = 0x04,
}

[FlagsAttribute]
enum DirectSoundPlayFlags : uint
{
    Looping = 0x01,
    LockHardware = 0x02,
    LockSoftware = 0x04,
    TerminateByTime = 0x08,
    TerminateByDistance = 0x10,
    TerminateByPriority = 0x20,
}

enum DirectSoundBufferLockFlags : uint
{
    None = 0x00,
    FromWriteCursor = 0x01,
    EntireBuffer = 0x02,
}

[FlagsAttribute]
enum DirectSoundBufferStatus : uint
{
    Playing = 0x01,
    BufferLost = 0x02,
    Looping = 0x04,
    LockHardware = 0x08,
    LockSoftware = 0x10,
    Terminate = 0x20,
}

[FlagsAttribute]
enum DirectSoundBufferCaps : uint
{
    PrimaryBuffer = 0x01,
    Static = 0x02,
    LockHardware = 0x04,
    LockSoftware = 0x08,
    Ctrl3d = 0x10,
    CtrlFrequency = 0x20,
    CtrlPan = 0x40,
    CtrlVolume = 0x80,
    CtrlPositionNotify = 0x100,
    CtrlFx = 0x200,
    StickyFocus = 0x4000,
    GlobalFocus = 0x8000,
    GetCurrentPosition2 = 0x10000,
    Mute3dAtMaxDistance = 0x20000,
    LockDefer = 0x40000,
}

[StructLayout(LayoutKind.Sequential)]
struct DirectSoundBufferPositionNotify
{
    public uint dwOffset;
    public IntPtr hEventNotify;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
struct BufferDescription
{
    public int dwSize;

    [MarshalAs(UnmanagedType.U4)]
    public DirectSoundBufferCaps dwFlags;
    public uint dwBufferBytes;
    public int dwReserved;
    public IntPtr lpWxfFormat;
    public Guid guid;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
struct BufferCaps
{
    public int dwSize;
    public int dwFlags;
    public int dwBufferBytes;
    public int dwUnlockTransferRate;
    public int dwPlayCpuOverhead;
}

[
    ComImport,
    Guid("279AFA83-4981-11CE-A521-0020AF0BE560"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    SuppressUnmanagedCodeSecurity
]
interface IDirectSound
{
    void CreateSoundBuffer(
        [In] BufferDescription desc,
        [Out, MarshalAs(UnmanagedType.Interface)] out object dsDSoundBuffer,
        IntPtr pUnkOuter
    );

    void GetCaps(IntPtr caps);

    void DuplicateSoundBuffer(
        [In, MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferOriginal,
        [In, MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer bufferDuplicate
    );

    void SetCooperativeLevel(
        IntPtr hwnd,
        [In, MarshalAs(UnmanagedType.U4)] DirectSoundCooperativeLevel dwLevel
    );
    void Compact();
    void GetSpeakerConfig(IntPtr pwdSpeakerConfig);
    void SetSpeakerConfig(IntPtr pwdSpeakerConfig);
    void Initialize([In, MarshalAs(UnmanagedType.LPStruct)] Guid guid);
}

[
    ComImport,
    Guid("279AFA85-4981-11CE-A521-0020AF0BE560"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    SuppressUnmanagedCodeSecurity
]
interface IDirectSoundBuffer
{
    void GetCaps([MarshalAs(UnmanagedType.LPStruct)] BufferCaps pBufferCaps);
    void GetCurrentPosition([Out] out uint currentPlayCursor, [Out] out uint currentWriteCursor);
    void GetFormat();

    [return: MarshalAs(UnmanagedType.I4)]
    int GetVolume();

    void GetPan([Out] out uint pan);

    [return: MarshalAs(UnmanagedType.I4)]
    int GetFrequency();

    [return: MarshalAs(UnmanagedType.U4)]
    DirectSoundBufferStatus GetStatus();

    void Initialize(
        [In, MarshalAs(UnmanagedType.Interface)] IDirectSound directSound,
        [In] BufferDescription desc
    );

    void Lock(
        int dwOffset,
        uint dwBytes,
        [Out] out IntPtr audioPtr1,
        [Out] out int audioBytes1,
        [Out] out IntPtr audioPtr2,
        [Out] out int audioBytes2,
        [MarshalAs(UnmanagedType.U4)] DirectSoundBufferLockFlags dwFlags
    );

    void Play(
        uint dwReserved1,
        uint dwPriority,
        [In, MarshalAs(UnmanagedType.U4)] DirectSoundPlayFlags dwFlags
    );
    void SetCurrentPosition(uint dwNewPosition);
    void SetFormat([In] WaveFormat pcfxFormat);
    void SetVolume(int volume);
    void SetPan(uint pan);
    void SetFrequency(uint frequency);
    void Stop();
    void Unlock(IntPtr pvAudioPtr1, int dwAudioBytes1, IntPtr pvAudioPtr2, int dwAudioBytes2);
    void Restore();
}

[
    ComImport,
    Guid("b0210783-89cd-11d0-af08-00a0c925cd16"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    SuppressUnmanagedCodeSecurity
]
interface IDirectSoundNotify
{
    void SetNotificationPositions(
        uint dwPositionNotifies,
        [In, MarshalAs(UnmanagedType.LPArray)] DirectSoundBufferPositionNotify[] pcPositionNotifies
    );
}

internal delegate bool DsEnumCallback(
    IntPtr lpGuid,
    IntPtr lpcstrDescription,
    IntPtr lpcstrModule,
    IntPtr lpContext
);

struct DirectSoundDeviceInfo
{
    public Guid guid;
    public string? description;
    public string? moduleName;
}
