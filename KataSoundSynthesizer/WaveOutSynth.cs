#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

class WaveOutSynth
{
    private IntPtr hWaveOut;
    private WaveOutBuffer[]? buffers;
    private IWaveStream? waveStream;
    private readonly WaveCallback waveCallback;
    private readonly WaveCallbackInfo waveCallbackInfo;
    private readonly object waveOutLock;
    private int queuedBuffers;

    public event EventHandler<StoppedEventData>? Stopped;

    public static WaveOutCapabilities GetCapabilities(int deviceNumber)
    {
        var caps = new WaveOutCapabilities();

        try
        {
            WinMultiMediaApi.waveOutGetDevCaps(
                (IntPtr)deviceNumber,
                out caps,
                Marshal.SizeOf(caps)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return caps;
    }

    public static int GetDeviceCount()
    {
        return WinMultiMediaApi.waveOutGetNumDevs();
    }

    public int DesiredLatency { get; set; }
    public int NumberOfBuffers { get; set; }
    public int DeviceNumber { get; set; }

    public WaveOutSynth()
    {
        DeviceNumber = 0;
        DesiredLatency = 300;
        NumberOfBuffers = 2;

        waveCallback = Callback;
        waveOutLock = new object();
        waveCallbackInfo = WaveCallbackInfo.FunctionCallback();
        waveCallbackInfo.Connect(waveCallback);
    }

    public void Init(IWaveStream waveStream)
    {
        this.waveStream = waveStream;

        var bufferSize = this.waveStream.WaveFormat.ConvertLatencyToByteSize(
            (DesiredLatency + NumberOfBuffers - 1) / NumberOfBuffers
        );

        MmResult result;
        lock (waveOutLock)
        {
            result = waveCallbackInfo.WaveOutOpen(
                out hWaveOut,
                DeviceNumber,
                this.waveStream.WaveFormat,
                waveCallback
            );
        }

        buffers = new WaveOutBuffer[NumberOfBuffers];
        for (var i = 0; i < buffers.Length; ++i)
        {
            buffers[i] = new WaveOutBuffer(hWaveOut, bufferSize, this.waveStream, waveOutLock);
        }

        SetWaveOutVolume(1.0f, hWaveOut, waveOutLock);
    }

    public void Play()
    {
        EnqueueBuffers();
    }

    private void EnqueueBuffers()
    {
        for (var i = 0; i < NumberOfBuffers; ++i)
        {
            if (buffers != null && !buffers[i].InQueue)
            {
                if (buffers[i].OnDone())
                {
                    Interlocked.Increment(ref queuedBuffers);
                }
            }
        }
    }

    public void Stop()
    {
        MmResult result;
        lock (waveOutLock)
        {
            result = WinMultiMediaApi.waveOutReset(hWaveOut);
        }

        if (result != MmResult.NoError)
        {
            throw new Exception("waveOutReset");
        }
    }

    private static void SetWaveOutVolume(float value, IntPtr hWaveOut, object lockObject)
    {
        if (value < 0 || value > 1)
        {
            throw new ArgumentOutOfRangeException("value", "Volume must be between 0.0 and 1.0");
        }

        var left = value;
        var right = value;

        var stereoVolume = (int)(left * 0xFFFF) + ((int)(right * 0xFFFF) << 16);
        MmResult result;
        lock (lockObject)
        {
            result = WinMultiMediaApi.waveOutSetVolume(hWaveOut, stereoVolume);
        }
    }

    private void Callback(
        IntPtr hWaveOut,
        WaveMessage uMsg,
        IntPtr dwDistance,
        WaveHeader? wavHdr,
        IntPtr dwReseverd
    )
    {
        if (uMsg != WaveMessage.WaveOutDone)
        {
            return;
        }

        if (wavHdr == null)
        {
            return;
        }

        var hBuffer = (GCHandle)wavHdr.userData;
        WaveOutBuffer? buffer = (WaveOutBuffer?)hBuffer.Target;
        Interlocked.Decrement(ref queuedBuffers);
        Exception? error = null;

        lock (waveOutLock)
        {
            try
            {
                if (buffer != null && buffer.OnDone())
                {
                    Interlocked.Increment(ref queuedBuffers);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }

        if (queuedBuffers == 0 && error != null)
        {
            RaiseStoppedEvent(error);
        }
    }

    private void RaiseStoppedEvent(Exception e)
    {
        if (Stopped != null)
        {
            Stopped(this, new StoppedEventData(e));
        }
    }

    public void Dispose()
    {
        if (buffers != null)
        {
            foreach (var b in buffers)
            {
                if (b != null)
                {
                    b.Dispose();
                }
            }

            buffers = null;
        }

        lock (waveOutLock)
        {
            WinMultiMediaApi.waveOutClose(hWaveOut);
        }

        waveCallbackInfo.Disconnect();
    }
}
