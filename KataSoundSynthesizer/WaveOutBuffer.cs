#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

class WaveOutBuffer : IDisposable
{
    private readonly WaveHeader waveHeader;
    private readonly Int32 bufferSize;
    private readonly byte[] buffer;
    private readonly IWaveStream waveStream;
    private readonly object waveOutLock;

    private GCHandle hBuffer;
    private IntPtr hWaveOut;
    private GCHandle hHeader;
    private GCHandle hThis;

    public Int32 BufferSize
    {
        get { return bufferSize; }
    }

    public WaveOutBuffer(
        IntPtr hWaveOut,
        Int32 bufferSize,
        IWaveStream waveStream,
        object waveOutLock
    )
    {
        this.bufferSize = bufferSize;
        buffer = new byte[bufferSize];
        hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        this.hWaveOut = hWaveOut;
        this.waveStream = waveStream;
        this.waveOutLock = waveOutLock;

        waveHeader = new WaveHeader();
        hHeader = GCHandle.Alloc(waveHeader, GCHandleType.Pinned);
        waveHeader.dataBuffer = hBuffer.AddrOfPinnedObject();
        waveHeader.bufferLength = bufferSize;
        waveHeader.loops = 1;
        hThis = GCHandle.Alloc(this);
        waveHeader.userData = (IntPtr)hThis;

        lock (this.waveOutLock)
        {
            try
            {
                WinMultiMediaApi.waveOutPrepareHeader(
                    hWaveOut,
                    waveHeader,
                    Marshal.SizeOf(waveHeader)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Dispose()
    {
        if (hHeader.IsAllocated)
        {
            hHeader.Free();
        }

        if (hBuffer.IsAllocated)
        {
            hBuffer.Free();
        }

        if (hThis.IsAllocated)
        {
            hThis.Free();
        }

        if (hWaveOut != IntPtr.Zero)
        {
            lock (waveOutLock)
            {
                WinMultiMediaApi.waveOutUnprepareHeader(
                    hWaveOut,
                    waveHeader,
                    Marshal.SizeOf(waveHeader)
                );
            }
            hWaveOut = IntPtr.Zero;
        }
    }

    public bool OnDone()
    {
        var bytes = 0;
        lock (waveStream)
        {
            bytes = waveStream.Read(buffer, 0, buffer.Length);
        }

        if (bytes == 0)
        {
            return false;
        }

        for (var i = bytes; i < buffer.Length; ++i)
        {
            buffer[i] = 0;
        }

        WriteToWaveOut();
        return true;
    }

    public bool InQueue
    {
        get { return (waveHeader.flags & WaveHeaderFlags.InQueue) == WaveHeaderFlags.InQueue; }
    }

    private void WriteToWaveOut()
    {
        MmResult result;
        lock (waveOutLock)
        {
            result = WinMultiMediaApi.waveOutWrite(
                hWaveOut,
                waveHeader,
                Marshal.SizeOf(waveHeader)
            );
        }

        if (result != MmResult.NoError)
        {
            throw new Exception("waveOutWrite");
        }

        GC.KeepAlive(this);
    }
}
