#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;
using KataSoundSynthesizer.Tone;
using KataSoundSynthesizer.Wave;
using NUnit.Framework;

namespace KataSoundSynthesizer.Oscillators;

[TestFixture]
public class OscillatorWaveStreamTest
{
    private ManualResetEvent waitHandle = null!;
    private const int Timeout20Seconds = 20000;
    private const int SampleRate16K = 16000;
    private const int Channels2 = 2;
    private const int Duration2Seconds = 2;

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void RenderSamples_WhenSine_PlaySineWave()
    {
        var scale = new Scale();
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[40], // middle C
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenSquare_PlaySquareWave()
    {
        var scale = new Scale();
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Square,
            Frequency = (float)scale.Tones[40], // middle C
            Amplitude = 0.05f,
            SampleRate = SampleRate16K,
        };
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenTriangle_PlayTriangleWave()
    {
        var scale = new Scale();
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Triangle,
            Frequency = (float)scale.Tones[40], // middle C
            Amplitude = 0.5f,
            SampleRate = SampleRate16K,
        };
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenSawtooth_PlaySawtoothWave()
    {
        var scale = new Scale();
        var osc = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sawtooth,
            Frequency = (float)scale.Tones[40], // middle C
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenNoise_PlayNoise()
    {
        var scale = new Scale();
        var osc = new Oscillator2 { WaveForm = WaveFormEnum.Noise, Amplitude = 0.05f };
        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenComposite_ThenPlayChord()
    {
        const int middleC = 40;
        const int middleE = 44;
        const int middleG = 47;
        var scale = new Scale();
        var compositeOsc = new CompositeOscillator();

        var osc1 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleC],
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        compositeOsc.AddOscillator(osc1);

        var osc2 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleE],
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        compositeOsc.AddOscillator(osc2);

        var osc3 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleG],
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        compositeOsc.AddOscillator(osc3);

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, compositeOsc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenFmModulator_ThenModulateOutput()
    {
        var scale = new Scale();
        var fmModulator = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 2.0f,
            Amplitude = 1.0f,
            SampleRate = SampleRate16K,
        };

        const bool useFmModulator_On = true;
        var osc = new Oscillator2(fmModulator, useFmModulator_On)
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[40], // middle C
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
            FmLevel = 0.10f,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenStateVariableFilter_ThenFilterOutput()
    {
        const int middleC = 40;
        var scale = new Scale();
        var osc1 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Square,
            Frequency = (float)scale.Tones[middleC],
            SampleRate = SampleRate16K,
        };

        var osc2 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sawtooth,
            Frequency = (float)scale.Tones[middleC + 12],
            SampleRate = SampleRate16K,
        };

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.01f,
            DecayTime = 0.15f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.15f,
            VelocitySensitivity = 0.2f,
        };

        adsr.Trigger(middleC, 0.85f);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 4f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, adsr, lfo)
        {
            CutOffFrequency = (float)scale.Tones[middleC + 4],
            Resonance = 0.95f,
            Drive = 2.0f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.95f,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, filter, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenStateVariableFilter_WithNoise_ThenFilterOutput()
    {
        const int middleC = 40;
        var osc1 = new Oscillator2 { WaveForm = WaveFormEnum.Noise, SampleRate = SampleRate16K };

        var osc2 = new Oscillator2 { WaveForm = WaveFormEnum.Noise, SampleRate = SampleRate16K };

        var adsr = new AdsrEnvelope()
        {
            SampleRate = SampleRate16K,
            AttackTime = 0.01f,
            DecayTime = 0.15f,
            SustainLevel = 0.85f,
            ReleaseTime = 0.15f,
            VelocitySensitivity = 0.2f,
        };

        adsr.Trigger(middleC, 0.85f);

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 4f,
            SampleRate = SampleRate16K,
        };

        var filter = new StateVariableFilter(osc1, osc2, adsr, lfo)
        {
            CutOffFrequency = 440.0f,
            Resonance = 0.55f,
            Drive = 0.85f,
            Filter = FilterType.LowPass,
            FilterGain = 0.02f,
            Amplitude = 0.95f,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, filter, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenWaveFormOscillator_ThenPlayOutput()
    {
        const int middleC = 40;
        const string Soundbank = @"SoundBank";
        // const string EGuitar0001 = @"AKWF_eguitar_0001.wav";
        const string EPiano0001 = @"AKWF_epiano_0001.wav";

        var waveFormFile = EPiano0001;
        var waveData = FileReader.Read(
            System.IO.Path.Combine(CurrentDirectory, Soundbank, waveFormFile)
        );
        if (waveData == null)
        {
            Assert.Fail("wave data is null");
            return;
        }

        var waveBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);
        var fmModulator = new Oscillator2 { Frequency = 8f, SampleRate = waveData.SampleRate };
        var slew = new Slew();
        var osc = new WaveFormOscillator(waveBuffer, fmModulator, slew)
        {
            Frequency = middleC,
            SampleRate = waveData.SampleRate,
            Amplitude = 0.05f,
            Phase = 1f,
            FmLevel = 0.0f,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(waveData.SampleRate, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenOctaveMultiple_ThenPlayOutput()
    {
        const int middleC = 40;
        var scale = new Scale();
        var compositeOsc = new CompositeOscillator();

        var osc1 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleC],
            SampleRate = SampleRate16K,
        };
        compositeOsc.AddOscillator(osc1);

        var osc2 = new Oscillator2
        {
            WaveForm = WaveFormEnum.Triangle,
            Frequency = (float)scale.Tones[middleC],
            Multiplier = OctaveMultiplier.x4,
            Amplitude = 0.25f,
            SampleRate = SampleRate16K,
        };
        compositeOsc.AddOscillator(osc2);

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, compositeOsc, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_WhenDutyCycleModulator_ThenPlayOutput()
    {
        const int middleC = 40;
        var scale = new Scale();

        var lfo = new Oscillator2
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = 0.25f,
            Amplitude = 0.00015f,
            SampleRate = SampleRate16K,
        };

        var osc1 = new Oscillator2(null, lfo)
        {
            WaveForm = WaveFormEnum.Square,
            Frequency = (float)scale.Tones[middleC],
            Amplitude = 0.2f,
            SampleRate = SampleRate16K,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc1, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
    }

    [Test]
    public void RenderSamples_FmModulation_ThenPlayOutput()
    {
        const int middleC = 40;
        var scale = new Scale();

        var fmModulator = new Oscillator2()
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleC - 24],
            Detune = 20.0f,
            Amplitude = 0.5f,
            SampleRate = SampleRate16K,
        };

        var osc1 = new Oscillator2(fmModulator, true)
        {
            WaveForm = WaveFormEnum.Sine,
            Frequency = (float)scale.Tones[middleC],
            Amplitude = 0.75f,
            FmLevel = 1f,
            SampleRate = SampleRate16K,
        };

        var waveFormat = WaveFormat.MakeIeeeFloatWaveFormat(SampleRate16K, Channels2);
        var waveStream = new OscillatorWaveStream(waveFormat, osc1, Duration2Seconds);
        PlayStream(waveStream, Timeout20Seconds);
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
