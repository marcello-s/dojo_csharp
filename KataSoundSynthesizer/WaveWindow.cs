#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

class WaveWindowNative(WaveCallback waveCallback) : NativeWindow
{
    private readonly WaveCallback waveCallback = waveCallback;

    protected override void WndProc(ref Message m)
    {
        var message = (WaveMessage)m.Msg;
        switch (message)
        {
            case WaveMessage.WaveOutDone:
            case WaveMessage.WaveInData:
                var hOutputDevice = m.WParam;
                var waveHeader = new WaveHeader();
                Marshal.PtrToStructure(m.LParam, waveHeader);
                waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                break;
            case WaveMessage.WaveOutOpen:
            case WaveMessage.WaveOutClose:
            case WaveMessage.WaveInOpen:
            case WaveMessage.WaveInClose:
                waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}

class WaveWindow : Form
{
    private readonly WaveCallback _waveCallback;

    public WaveWindow(WaveCallback waveCallback)
    {
        _waveCallback = waveCallback;
    }

    protected override void WndProc(ref Message m)
    {
        var message = (WaveMessage)m.Msg;
        switch (message)
        {
            case WaveMessage.WaveOutDone:
            case WaveMessage.WaveInData:
                var hOutputDevice = m.WParam;
                var waveHeader = new WaveHeader();
                Marshal.PtrToStructure(m.LParam, waveHeader);
                _waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                break;
            case WaveMessage.WaveOutOpen:
            case WaveMessage.WaveOutClose:
            case WaveMessage.WaveInOpen:
            case WaveMessage.WaveInClose:
                _waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
