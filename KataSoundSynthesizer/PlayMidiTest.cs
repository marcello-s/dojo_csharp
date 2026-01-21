#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Effect;
using KataSoundSynthesizer.Midi;
using KataSoundSynthesizer.Oscillators;
using KataSoundSynthesizer.SynthComponent;
using NUnit.Framework;
using FileReader = KataSoundSynthesizer.Midi.FileReader;

namespace KataSoundSynthesizer;

[TestFixture]
public class PlayMidiTest
{
    private ManualResetEvent waitHandle = null!;
    private const int Timeout1Minute = 60000;
    private const int Timeout5Minutes = 60000 * 5;
    private const string TheEntertainerFilePath = @"Midi\the_entertainer.mid";
    private const string ClaireDeLuneFilePath = @"Midi\deb_clai.mid";
    private const string DrumSampleFilePath = @"Midi\drum_sample.mid";
    private const string TetrisFilePath = @"Midi\tetris.mid";
    private const string DjangoFilePath = @"Midi\django.mid";
    private const string BttfPianoFilePath = @"Midi\bttf_piano.mid";
    private const int SampleRate16K = 16000;
    private const int SampleRate44K = 44100;
    private const int Channels2 = 2;
    private const int Duration35Seconds = 35;
    private const int Duration45Seconds = 45;
    private const int Duration150Seconds = 150;
    private const int Duration250Seconds = 250;

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void Play_The_Entertainer()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, TheEntertainerFilePath),
            out midiInfo
        );
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate16K, -2);
        var trackedKeys = mtr.Read(tracks);
        var voice = CreateVoice();

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration45Seconds + 1,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout1Minute);
    }

    [Test]
    public void Play_Claire_de_Lune()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, ClaireDeLuneFilePath),
            out midiInfo
        );
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate16K, -2);
        var trackedKeys = mtr.Read(tracks);
        var voice = CreateVoice();

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration250Seconds,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout5Minutes);
    }

    [Test]
    public void Play_Drum_Sample()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, DrumSampleFilePath),
            out midiInfo
        );
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate44K, 0);
        var trackedKeys = mtr.Read(tracks);

        foreach (var trackedKey in trackedKeys)
        {
            Console.WriteLine(
                "dt:{0} ch:{1} note:{2} v:{3} mode:{4}",
                trackedKey.DeltaTimeTicks,
                trackedKey.Channel,
                trackedKey.NoteNumber,
                trackedKey.Velocity,
                trackedKey.Mode
            );
        }

        //var percussionVoice = new Percussions();
        //var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate44K, Channels2);
        //var waveStream = new VoiceWaveStream(waveFormat, percussionVoice, Duration45Seconds, trackedKeys.ToArray());
        //PlayStream(waveStream, Timeout1Minute);
    }

    [Test]
    public void Play_Tetris()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(Path.Combine(CurrentDirectory, TetrisFilePath), out midiInfo);
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate16K, -1);
        var trackedKeys = mtr.Read(tracks);
        var voice = CreateVoice();

        foreach (var trackedKey in trackedKeys)
        {
            Console.WriteLine(
                "dt:{0} ch:{1} note:{2} v:{3} mode:{4}",
                trackedKey.DeltaTimeTicks,
                trackedKey.Channel,
                trackedKey.NoteNumber,
                trackedKey.Velocity,
                trackedKey.Mode
            );
        }

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            2 * Duration35Seconds + 1,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout1Minute);
    }

    [Test]
    public void Play_Django()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(Path.Combine(CurrentDirectory, DjangoFilePath), out midiInfo);
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate16K, -1);
        var trackedKeys = mtr.Read(tracks);
        var voice = CreateVoice();

        foreach (var trackedKey in trackedKeys)
        {
            Console.WriteLine(
                "dt:{0} ch:{1} note:{2} v:{3} mode:{4}",
                trackedKey.DeltaTimeTicks,
                trackedKey.Channel,
                trackedKey.NoteNumber,
                trackedKey.Velocity,
                trackedKey.Mode
            );
        }

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration150Seconds + 3,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout1Minute);
    }

    [Test]
    public void Play_BttfPiano()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, BttfPianoFilePath),
            out midiInfo
        );
        if (tracks == null)
        {
            Assert.Fail("Could not read midi file");
            return;
        }

        var mtr = new MidiTrackReader(SampleRate16K, -2);
        var trackedKeys = mtr.Read(tracks);
        var voice = CreateVoice();

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration35Seconds,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout1Minute);
    }

    private static IVoice CreateVoice()
    {
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Triangle, Detune = -0.0001f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = 0.0001f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.SampleRate = SampleRate16K;
        osc.Amplitude = 0.85f;
        osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.015f,
            DecayTime = 0.35f,
            SustainLevel = 0.45f,
            ReleaseTime = 0.25f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew();

        var adsrEnvelope = new CompositeAdsrEnvelope();
        adsrEnvelope.AddAdsrEnvelope(adsr);
        adsrEnvelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 2.0f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, adsrEnvelope, lfo)
        {
            CutOffFrequency = 2400f,
            Resonance = 0.05f,
            Drive = 0.5f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        var effect = new Chorus
        {
            Delay = 0.5f,
            Depth = 0.003f,
            FeedbackLevel = 0.25f,
            Frequency = 2.0f,
            SampleRate = SampleRate16K,
        };

        //IEffectComponent effect = null;

        var voice = new Voice(filter, osc, adsrEnvelope, lfo, 0.85f, 0.1f, 0.25f, effect, true);
        return new PolyVoice(voice);
    }

    private void PlayStream(IWaveStream waveStream, int timeoutInMilliseconds)
    {
        waitHandle = new ManualResetEvent(false);
        var waveOutSynth = new WaveOutSynth();
        waveOutSynth.Stopped += OnWaveOutSynthStopped;

        waveOutSynth.Init(waveStream);
        waveOutSynth.Play();

        var timeout = waitHandle.WaitOne(timeoutInMilliseconds);
        if (!timeout)
        {
            Console.WriteLine("playback timeout");
            waveOutSynth.Stop();
        }

        waveOutSynth.Dispose();
    }

    private void OnWaveOutSynthStopped(object? sender, StoppedEventData e)
    {
        if (waitHandle != null)
        {
            waitHandle.Set();
        }
    }
}
