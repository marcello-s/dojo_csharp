#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Effect;
using KataSoundSynthesizer.Oscillators;
using KataSoundSynthesizer.Tone;
using KataSoundSynthesizer.Wave;
using NUnit.Framework;

namespace KataSoundSynthesizer.SynthComponent;

[TestFixture]
public class VoiceWaveStreamTest
{
    private ManualResetEvent waitHandle = null!;
    private const int Timeout20Seconds = 20000;
    private const int SampleRate16K = 16000;
    private const int SampleRate44K = 44100;
    private const int Channels2 = 2;
    private const int Duration4Seconds = 4;
    private const int Duration6Seconds = 6;

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void RenderSamples_WhenVoice_ThenPlayVoice()
    {
        const int middleC = 40;
        var scale = new Scale();
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = -0.03f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = 0.03f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.Frequency = (float)scale.Tones[middleC];
        osc.SampleRate = SampleRate16K;
        osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.01f,
            DecayTime = 0.15f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.15f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew();

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 6.0f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = (float)scale.Tones[middleC + 4],
            Resonance = 0.95f,
            Drive = 2.0f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        const long durationInSamples = Duration4Seconds * SampleRate16K;
        var trackedKeys = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC, 0.85f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 6, middleC, 0.85f, 0),
            new TrackedKey(
                TrackedKey.KeyMode.Trigger,
                (durationInSamples / 4) - (durationInSamples / 6),
                middleC + 7,
                0.5f,
                0
            ),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 2, middleC + 7, 0.5f, 0),
        };

        var effect = new Echo
        {
            LeftDelayTime = 0.22f,
            RightDelayTime = 0.27f,
            SampleRate = SampleRate16K,
        };

        var voice = new Voice(filter, osc, envelope, lfo, 0.85f, 0.1f, 0.25f, effect, false);

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(waveFormat, voice, Duration4Seconds + 1, trackedKeys);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenVoiceWithChorus_ThenPlayVoice()
    {
        const int middleC = 40;
        var scale = new Scale();
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sawtooth, Detune = -0.03f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Triangle, Detune = 0.03f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.Frequency = (float)scale.Tones[middleC];
        osc.SampleRate = SampleRate16K;
        //osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.15f,
            DecayTime = 0.75f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.25f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew { SlewTime = 0.025f };

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 0.5f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = 2500f, // (float)scale.Tones[middleC + 14],
            Resonance = 0.25f,
            Drive = 0.5f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        const long durationInSamples = Duration4Seconds * SampleRate16K;
        var trackedKeys = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC, 0.85f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 6, middleC, 0.85f, 0),
            new TrackedKey(
                TrackedKey.KeyMode.Trigger,
                (durationInSamples / 4) - (durationInSamples / 6),
                middleC + 7,
                0.5f,
                0
            ),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 2, middleC + 7, 0.5f, 0),
        };

        var effect = new Chorus
        {
            Delay = 0.5f,
            Depth = 0.003f,
            FeedbackLevel = 0.85f,
            Frequency = 2.0f,
            SampleRate = SampleRate16K,
        };

        var voice = new Voice(filter, osc, envelope, lfo, 0.85f, 0.1f, 0.25f, effect, false);

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(waveFormat, voice, Duration4Seconds, trackedKeys);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WithReverb_ThenPlayVoice()
    {
        const int middleC = 40;
        var scale = new Scale();
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = -0.03f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = 0.03f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.Frequency = (float)scale.Tones[middleC];
        osc.SampleRate = SampleRate16K;
        osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.01f,
            DecayTime = 0.15f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.15f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew();

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 6.0f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = (float)scale.Tones[middleC + 4],
            Resonance = 0.95f,
            Drive = 2.0f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        const long durationInSamples = Duration4Seconds * SampleRate16K;
        var trackedKeys = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC, 0.85f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 6, middleC, 0.85f, 0),
            new TrackedKey(
                TrackedKey.KeyMode.Trigger,
                (durationInSamples / 4) - (durationInSamples / 6),
                middleC + 7,
                0.5f,
                0
            ),
            new TrackedKey(TrackedKey.KeyMode.Release, durationInSamples / 2, middleC + 7, 0.5f, 0),
        };

        var effect = new Reverb();

        var voice = new Voice(filter, osc, envelope, lfo, 0.85f, 0.1f, 0.25f, effect, false);

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(waveFormat, voice, Duration4Seconds + 1, trackedKeys);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_PlaySequence()
    {
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sawtooth, Detune = -0.03f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = 0.03f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.SampleRate = SampleRate16K;
        osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.01f,
            DecayTime = 0.2f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.15f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew();

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 2.0f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = 1400f,
            Resonance = 0.25f,
            Drive = 1.5f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        var effect = new Chorus
        {
            Delay = 0.5f,
            Depth = 0.0003f,
            FeedbackLevel = 0.35f,
            Frequency = 2.0f,
            SampleRate = SampleRate16K,
        };

        var voice = new Voice(filter, osc, envelope, lfo, 0.85f, 0.01f, 0.1f, effect, false);
        var polyVoice = new PolyVoice(voice);

        // microseconds=240000
        // cmd=NoteOn, ch=1, key=98, vel=91,  dt=0
        // cmd=NoteOn, ch=1, key=98, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=100, vel=98,  dt=1
        // cmd=NoteOn, ch=1, key=100, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=96, vel=86,  dt=1
        // cmd=NoteOn, ch=1, key=96, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=93, vel=84,  dt=1
        // cmd=NoteOn, ch=1, key=93, vel=0,  dt=255
        // cmd=NoteOn, ch=1, key=95, vel=103,  dt=1
        // cmd=NoteOn, ch=1, key=95, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=91, vel=85,  dt=1
        // cmd=NoteOn, ch=1, key=91, vel=0,  dt=139

        // cmd=NoteOn, ch=1, key=86, vel=86,  dt=117
        // cmd=NoteOn, ch=1, key=86, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=88, vel=99,  dt=1
        // cmd=NoteOn, ch=1, key=88, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=84, vel=84,  dt=1
        // cmd=NoteOn, ch=1, key=84, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=81, vel=84,  dt=1
        // cmd=NoteOn, ch=1, key=81, vel=0,  dt=255
        // cmd=NoteOn, ch=1, key=83, vel=103,  dt=1
        // cmd=NoteOn, ch=1, key=83, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=79, vel=84,  dt=1
        // cmd=NoteOn, ch=1, key=79, vel=0,  dt=139

        // cmd=NoteOn, ch=1, key=74, vel=112,  dt=117
        // cmd=NoteOn, ch=1, key=74, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=76, vel=102,  dt=1
        // cmd=NoteOn, ch=1, key=76, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=72, vel=88,  dt=1
        // cmd=NoteOn, ch=1, key=72, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=69, vel=86,  dt=1
        // cmd=NoteOn, ch=1, key=69, vel=0,  dt=255
        // cmd=NoteOn, ch=1, key=71, vel=102,  dt=1
        // cmd=NoteOn, ch=1, key=71, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=69, vel=91,  dt=1
        // cmd=NoteOn, ch=1, key=69, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=68, vel=89,  dt=1
        // cmd=NoteOn, ch=1, key=68, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=67, vel=119,  dt=1
        // cmd=NoteOn, ch=1, key=67, vel=0,  dt=139

        // cmd=NoteOn, ch=1, key=71, vel=107,  dt=373
        // cmd=NoteOn, ch=1, key=79, vel=106,  dt=0
        // cmd=NoteOn, ch=1, key=43, vel=90,  dt=0

        // cmd=NoteOn, ch=1, key=71, vel=0,  dt=126
        // cmd=NoteOn, ch=1, key=79, vel=0,  dt=0
        // cmd=NoteOn, ch=1, key=43, vel=0,  dt=0

        // cmd=NoteOn, ch=1, key=62, vel=72,  dt=130
        // cmd=NoteOn, ch=1, key=62, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=63, vel=96,  dt=1
        // cmd=NoteOn, ch=1, key=63, vel=0,  dt=127

        // cmd=NoteOn, ch=1, key=64, vel=104,  dt=1
        // cmd=NoteOn, ch=1, key=48, vel=46,  dt=0
        // cmd=NoteOn, ch=1, key=64, vel=0,  dt=127
        // cmd=NoteOn, ch=1, key=72, vel=114,  dt=1

        var trackedKeys = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(0), Transpose(98), 0.91f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(98), 0.91f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(100), 0.98f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(100), 0.91f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(96), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(96), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(93), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(255), Transpose(93), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(95), 1.03f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(95), 1.03f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(91), 0.85f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(139), Transpose(91), 0.85f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(117), Transpose(86), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(86), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(88), 0.99f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(88), 0.99f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(84), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(84), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(81), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(255), Transpose(81), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(83), 1.03f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(83), 1.03f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(79), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(139), Transpose(79), 0.84f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(117), Transpose(74), 1.12f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(74), 1.12f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(76), 1.02f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(76), 1.02f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(72), 0.88f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(84), 0.88f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(69), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(255), Transpose(69), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(71), 1.02f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(71), 1.02f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(69), 0.91f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(69), 0.91f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(68), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(127), Transpose(68), 0.86f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(1), Transpose(67), 1.19f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicks(139), Transpose(67), 1.19f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(373), Transpose(71), 1.07f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(0), Transpose(79), 1.06f, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicks(10), Transpose(43), 0.90f, 0),
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(waveFormat, polyVoice, Duration6Seconds, trackedKeys);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_PlayScale()
    {
        const int middleC = 40;
        const long durationInSamples = Duration6Seconds * SampleRate16K;
        const long beat = durationInSamples / 12;
        const float velocityProgress = 0.05f;
        var trackedKeys = new List<TrackedKey>();
        var time = 0L;
        var velocity = velocityProgress;
        for (var i = 0; i < 12; ++i)
        {
            trackedKeys.Add(
                new TrackedKey(
                    TrackedKey.KeyMode.Trigger,
                    time,
                    middleC + i,
                    velocity += velocityProgress,
                    0
                )
            );
            trackedKeys.Add(
                new TrackedKey(TrackedKey.KeyMode.Release, time + beat - 1, middleC + i, 0f, 0)
            );

            if (i == 0)
            {
                time = 1;
            }
        }

        var voice = CreateSimpleVoice();
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration6Seconds + 1,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_PlayOctave()
    {
        const int middleC = 40;
        const long durationInSamples = Duration4Seconds * SampleRate16K;
        const long beat = durationInSamples / 8;
        const float velocityProgress = 0.05f;
        var trackedKeys = new List<TrackedKey>();
        var time = 0L;
        var velocity = velocityProgress;
        for (var i = 0; i < 3; ++i)
        {
            trackedKeys.Add(
                new TrackedKey(
                    TrackedKey.KeyMode.Trigger,
                    time,
                    middleC + 2 * i,
                    velocity += velocityProgress,
                    0
                )
            );
            trackedKeys.Add(
                new TrackedKey(TrackedKey.KeyMode.Release, time + beat - 1, middleC + 2 * i, 0f, 0)
            );

            if (i == 0)
            {
                time = 1;
            }
        }

        for (var i = 0; i < 4; ++i)
        {
            trackedKeys.Add(
                new TrackedKey(
                    TrackedKey.KeyMode.Trigger,
                    time,
                    middleC + 5 + 2 * i,
                    velocity += velocityProgress,
                    0
                )
            );
            trackedKeys.Add(
                new TrackedKey(
                    TrackedKey.KeyMode.Release,
                    time + beat - 1,
                    middleC + 5 + 2 * i,
                    0f,
                    0
                )
            );
        }

        trackedKeys.Add(
            new TrackedKey(TrackedKey.KeyMode.Trigger, time, middleC + 12, velocity, 0)
        );
        trackedKeys.Add(
            new TrackedKey(TrackedKey.KeyMode.Release, time + 2 * beat - 1, middleC + 12, 0f, 0)
        );

        var voice = CreateSimpleVoice();
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration4Seconds + 1,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_PlayChords()
    {
        const int middleC = 40;
        const long durationInSamples = Duration4Seconds * SampleRate16K;
        const long beat = durationInSamples / 4;
        const float velocity = 0.75f;
        var trackedKeys = new List<TrackedKey>
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 7, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, beat, middleC, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 7, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, beat, middleC, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 12, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, beat, middleC + 4, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 12, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 5, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, 0, middleC + 12, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, beat, middleC + 5, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 9, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 0, middleC + 12, velocity, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, 2 * beat, middleC + 12, velocity, 0),
        };

        var voice = CreatePolyVoice();
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            voice,
            Duration4Seconds + 1,
            trackedKeys.ToArray()
        );
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WaveVoice_PlaySequence()
    {
        const string Soundbank = @"SoundBank";
        const string OpenHat003 = @"Open Hat 003.wav";

        var waveData = FileReader.Read(Path.Combine(CurrentDirectory, Soundbank, OpenHat003));
        if (waveData == null)
        {
            Assert.Fail("wave data is null");
            return;
        }

        var waveBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);

        var waveVoice = new WaveVoice(waveBuffer);

        var trackedKeys = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(0), 0, 1f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicksWave(4096), 0, 0, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 0, 0.75f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicksWave(8192), 0, 0, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 0, 1f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicksWave(4096), 0, 0, 0),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 0, 0.75f, 0),
            new TrackedKey(TrackedKey.KeyMode.Release, TimeTicksWave(8192), 0, 0, 0),
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate44K, Channels2);
        var waveStream = new VoiceWaveStream(waveFormat, waveVoice, Duration4Seconds, trackedKeys);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WaveVoicePercussions_PlayRockBeat()
    {
        const int beat = 2 * 1024;
        var rockSequence = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(2 * beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 36, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(2 * beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 38, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(2 * beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 36, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(2 * beat), 46, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 38, 1f, 9),
        };

        var sequence = new List<TrackedKey>(rockSequence);
        sequence.AddRange(rockSequence);
        var trackedKeys = sequence.ToArray();

        var percussionsVoice = new Percussions(CurrentDirectory);
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate44K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            percussionsVoice,
            Duration4Seconds,
            trackedKeys
        );
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WaveVoicePercussions_PlayAlternateBassBeat()
    {
        const int beat = 2 * 1024;
        var alternateBassBeat = new[]
        {
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 36, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 38, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 36, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 36, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(1), 38, 1f, 9),
            new TrackedKey(TrackedKey.KeyMode.Trigger, TimeTicksWave(beat), 42, 1f, 9),
        };

        var sequence = new List<TrackedKey>(alternateBassBeat);
        sequence.AddRange(alternateBassBeat);
        var trackedKeys = sequence.ToArray();

        var percussionsVoice = new Percussions(CurrentDirectory);
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate44K, Channels2);
        var waveStream = new VoiceWaveStream(
            waveFormat,
            percussionsVoice,
            Duration4Seconds,
            trackedKeys
        );
        PlayStream(waveStream, Timeout20Seconds);
    }

    private static long TimeTicksWave(int dt)
    {
        return 240000 / SampleRate44K * dt;
    }

    private static long TimeTicks(int dt)
    {
        return 240000 / SampleRate16K * dt;
    }

    private static int Transpose(int key)
    {
        return Math.Max(0, key - 2 * 12);
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

    private static IVoice CreateSimpleVoice()
    {
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = -0.04f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Sine, Detune = 0.04f };

        var osc = new CompositeOscillator();
        osc.AddOscillator(osc1);
        osc.AddOscillator(osc2);
        osc.SampleRate = SampleRate16K;
        osc.Amplitude = 0.85f;
        osc.CutOff = 0.85f;

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.15f,
            DecayTime = 0.75f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.25f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew { SlewTime = 0.025f };

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 0.5f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = 2400f, // (float)scale.Tones[middleC + 14],
            Resonance = 0.05f,
            Drive = 0.5f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        //var effect = new Echo
        //{
        //    LeftDelayTime = 0.22f,
        //    RightDelayTime = 0.27f,
        //    SampleRate = SampleRate16K
        //};

        //var effect = new Chorus
        //{
        //    Delay = 0.5f,
        //    Depth = 0.003f,
        //    FeedbackLevel = 0.85f,
        //    Frequency = 2.0f,
        //    SampleRate = SampleRate16K
        //};

        IEffectComponent? effect = null;

        return new Voice(filter, osc, envelope, lfo, 0.85f, 0.1f, 0.25f, effect, false);
    }

    private static IVoice CreatePolyVoice()
    {
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Sawtooth, Detune = -0.0004f };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Triangle, Detune = 0.0004f };

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
            DecayTime = 0.45f,
            SustainLevel = 0.45f,
            ReleaseTime = 0.25f,
            VelocitySensitivity = 0.2f,
        };

        var slew = new Slew { SlewTime = 0.025f };

        var envelope = new CompositeAdsrEnvelope();
        envelope.AddAdsrEnvelope(adsr);
        envelope.AddAdsrEnvelope(slew);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 0.5f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, envelope, lfo)
        {
            CutOffFrequency = 2500f, // (float)scale.Tones[middleC + 14],
            Resonance = 0.25f,
            Drive = 0.5f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.35f,
        };

        //var effect = new Echo
        //{
        //    LeftDelayTime = 0.22f,
        //    RightDelayTime = 0.27f,
        //    SampleRate = SampleRate16K
        //};

        //var effect = new Chorus
        //{
        //    Delay = 0.5f,
        //    Depth = 0.003f,
        //    FeedbackLevel = 0.85f,
        //    Frequency = 2.0f,
        //    SampleRate = SampleRate16K
        //};

        IEffectComponent? effect = null;

        var voice = new Voice(filter, osc, envelope, lfo, 0.85f, 0.1f, 0.25f, effect, false);
        return new PolyVoice(voice);
    }

    [Test]
    public void BufferingTest()
    {
        ConsumeData();
    }

    private void ConsumeData()
    {
        const int chunkSize = 64;
        var buffer = new int[chunkSize];
        while (Buffering(buffer, chunkSize) > 0)
        {
            Console.WriteLine(buffer.Length);
        }
    }

    private readonly int[] _vessel = new int[512];
    private int _vesselSize;
    private int _served;

    private int Buffering(int[] buffer, int count)
    {
        // serve
        while ((_vesselSize - _served) >= count)
        {
            Array.Copy(_vessel, _served, buffer, 0, count);
            _served += count;
            return count;
        }

        // fill
        while ((_vesselSize - _served) < count)
        {
            var data = ProduceData();
            if (data.Length <= 0)
            {
                Console.WriteLine("vesselSize: " + _vesselSize);
                Console.WriteLine("served: " + _served);
                return 0;
            }

            Array.Copy(data, 0, _vessel, _vesselSize, data.Length);
            _vesselSize += data.Length;
        }

        // re-arrange
        _vesselSize = _vesselSize - _served;
        Array.Copy(_vessel, _served, _vessel, 0, _vesselSize);
        _served = 0;

        // serve
        Array.Copy(_vessel, _served, buffer, 0, count);
        _served += count;
        return count;
    }

    private int _dataIndex;

    private int[] ProduceData()
    {
        var sizes = new[] { 32, 64, 256, 128, 64, 32, 64 };
        return _dataIndex < sizes.Length ? new int[sizes[_dataIndex++]] : new int[0];
    }
}
