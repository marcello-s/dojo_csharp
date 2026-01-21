#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Midi.Device;

[TestFixture]
public class DeviceListTest
{
    [Test]
    public void Winmm_MidiGetNumDevs()
    {
        var numberOfDevices = Winmm.midiInGetNumDevs();
        Console.WriteLine(numberOfDevices);
    }

    [Test]
    public void Winmm_MidiInOpen_MidiInClose()
    {
        Console.WriteLine($"message codes: open= {Winmm.MimOpen}, close= {Winmm.MimClose}");
        Console.WriteLine("open midi device..");
        var result = Winmm.midiInOpen(
            out var handle,
            0,
            HandleMessage,
            IntPtr.Zero,
            Winmm.CallbackFunction
        );
        Console.WriteLine($"result: {result}");
        if (result == 0)
        {
            System.Threading.Thread.Sleep(1000 * 5);
            Console.WriteLine("close midi device");
            result = Winmm.midiInClose(handle);
            Console.WriteLine($"result: {result}");
        }
    }

    [Test]
    public void Winmm_MidiInStart_MidiInStop()
    {
        Console.WriteLine($"message codes: open= {Winmm.MimOpen}, close= {Winmm.MimClose}");
        Console.WriteLine(
            $"message codes: data= {Winmm.MimData}, longdata= {Winmm.MimLongData}, moredata= {Winmm.MimMoreData}"
        );
        Console.WriteLine("open midi device..");
        var result = Winmm.midiInOpen(
            out var handle,
            0,
            HandleMessage,
            IntPtr.Zero,
            Winmm.CallbackFunction
        );
        Console.WriteLine($"result: {result}");
        Console.WriteLine($"handle: {handle.ToInt64()}");

        if (result == 0)
        {
            Console.WriteLine("start midi in..");
            result = Winmm.midiInStart(handle);
            Console.WriteLine($"result: {result}");
            if (result == 0)
            {
                System.Threading.Thread.Sleep(1000 * 10);
                Console.WriteLine("stop midi in..");
                result = Winmm.midiInStop(handle);
                Console.WriteLine($"result: {result}");
                Console.WriteLine("reset..");
                result = Winmm.midiInReset(handle);
                Console.WriteLine($"result: {result}");
            }

            result = Winmm.midiInClose(handle);
            Console.WriteLine($"result: {result}");
        }
    }

    private void HandleMessage(
        IntPtr handle,
        int message,
        IntPtr instance,
        IntPtr param1,
        IntPtr param2
    )
    {
        var param = new MidiParams(param1, param2);
        Console.WriteLine(
            $"message: {message}, param1: {param.Param1.ToInt32()}, param2: {param.Param2.ToInt32()}"
        );
    }
}
