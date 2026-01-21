#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer;

class WaveCallbackInfo
{
    private WaveWindow? waveOutWindow;
    private WaveWindowNative? waveOutWindowNative;

    public WaveCallbackStrategy Strategy { get; private set; }
    public IntPtr Handle { get; private set; }

    private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
    {
        Strategy = strategy;
        Handle = handle;
    }

    public static WaveCallbackInfo FunctionCallback()
    {
        return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
    }

    public static WaveCallbackInfo NewWindow()
    {
        return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
    }

    public static WaveCallbackInfo ExistingWindow(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentNullException("handle");
        }

        return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
    }

    public void Connect(WaveCallback waveCallback)
    {
        if (Strategy == WaveCallbackStrategy.NewWindow)
        {
            waveOutWindow = new WaveWindow(waveCallback);
            waveOutWindow.CreateControl();
            Handle = waveOutWindow.Handle;
        }
        else if (Strategy == WaveCallbackStrategy.ExistingWindow)
        {
            waveOutWindowNative = new WaveWindowNative(waveCallback);
            waveOutWindowNative.AssignHandle(Handle);
        }
    }

    public MmResult WaveOutOpen(
        out IntPtr waveOutHandle,
        int deviceNumber,
        WaveFormat waveFormat,
        WaveCallback waveCallback
    )
    {
        MmResult result;
        if (Strategy == WaveCallbackStrategy.FunctionCallback)
        {
            result = WinMultiMediaApi.waveOutOpen(
                out waveOutHandle,
                (IntPtr)deviceNumber,
                waveFormat,
                waveCallback,
                IntPtr.Zero,
                WaveInOutOpenFlags.CallbackFunction
            );
        }
        else
        {
            result = WinMultiMediaApi.waveOutOpenWindow(
                out waveOutHandle,
                (IntPtr)deviceNumber,
                waveFormat,
                Handle,
                IntPtr.Zero,
                WaveInOutOpenFlags.CallbackWindow
            );
        }

        return result;
    }

    public void Disconnect()
    {
        if (waveOutWindow != null)
        {
            waveOutWindow.Close();
            waveOutWindow = null!;
        }

        if (waveOutWindowNative != null)
        {
            waveOutWindowNative.ReleaseHandle();
            waveOutWindowNative = null!;
        }
    }
}

public enum WaveCallbackStrategy
{
    FunctionCallback,
    NewWindow,
    ExistingWindow,
    Event,
}
