#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Tone;

[TestFixture]
public class ScaleTest
{
    [Test]
    public void ToneFrequency_WhenProvidedBaseTone_ThenGetNewTone()
    {
        var scale = new Scale();
        var middleC = scale.ToneFrequency(Scale.A440, Scale.A440ToneIndex, 40);
        Assert.That(Math.Round(middleC, 3), Is.EqualTo(261.626));
    }

    [Test]
    public void Ctor_WhenNew_ThenGetAllTones()
    {
        var scale = new Scale();
        Assert.That(Math.Round(scale.Tones[Scale.A440ToneIndex - 12]), Is.EqualTo(220.0));
    }
}
