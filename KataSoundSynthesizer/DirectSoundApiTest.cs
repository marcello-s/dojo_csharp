#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer;

[TestFixture]
public class DirectSoundApiTest
{
    [Test, Explicit]
    public void GetDevices()
    {
        var devices = DirectSoundApi.GetDevices();

        foreach (var device in devices)
        {
            Console.WriteLine(
                "guid='{0}', desc='{1}', module='{2}'",
                device.guid,
                device.description,
                device.moduleName
            );
        }
    }

    [Test, Explicit]
    public void PlaybackLoop()
    {
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(16000, 2);
        var waveStream = new SweepWaveStream(waveFormat);
        var directSoundApi = new DirectSoundApi(waveStream);

        directSoundApi.PlaybackLoop();
        //System.Threading.Thread.Sleep(5000);
    }
}
