#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer;

[TestFixture]
public class NAudioPlayWaveTest
{
    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test, Explicit]
    public void PlaySineWave()
    {
        var player = new NAudioPlayWave();
        player.PlaySineWave();
    }

    [Test, Explicit]
    public void PlayWaveFile()
    {
        const string Soundbank = @"Soundbank";
        const string OpenHat003 = @"Open Hat 003.wav";
        var filename = Path.Combine(CurrentDirectory, Soundbank, OpenHat003);

        var player = new NAudioPlayWaveFile();
        player.PlayWaveFile(filename);
    }
}
