#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Wave;
using NUnit.Framework.Constraints;

namespace KataSoundSynthesizer.SynthComponent;

class Percussions : IVoice
{
    private const string SoundBank = @"SoundBank";
    private static readonly IEnumerable<VoiceWaveSample> VoiceWaveSamples =
        new List<VoiceWaveSample>
        {
            new VoiceWaveSample(Path.Combine(SoundBank, @"Kick 001 Basic.wav"), 36), // bass drum 1
            new VoiceWaveSample(Path.Combine(SoundBank, @"Snare 003.wav"), 38), // snare drum 1
            new VoiceWaveSample(Path.Combine(SoundBank, @"Closed Hat 008 Classic.wav"), 42), // closed Hi-hat
            new VoiceWaveSample(Path.Combine(SoundBank, @"Percussion Tom Drum 001.wav"), 43), // low tom 1
            new VoiceWaveSample(Path.Combine(SoundBank, @"Open Hat 003.wav"), 46), // Open Hi-hat
            new VoiceWaveSample(Path.Combine(SoundBank, @"Crash 001.wav"), 49), // Crash Cymbal 1
        };

    private readonly IDictionary<int, IVoice> voices;

    public float Panning { get; set; }

    public Percussions(string currentDirectory = "")
    {
        voices = new Dictionary<int, IVoice>();

        foreach (var voiceWaveSample in VoiceWaveSamples)
        {
            var waveData = FileReader.Read(
                Path.Combine(currentDirectory, voiceWaveSample.WaveSampleFilepath)
            );

            if (waveData == null)
            {
                throw new InvalidDataException(
                    $"Could not load percussion sample: {voiceWaveSample.WaveSampleFilepath}"
                );
            }

            var waveBuffer = WaveDataConverter.ConvertToFloatBuffer(waveData);
            voices.Add(voiceWaveSample.NoteNumber, new WaveVoice(waveBuffer));
        }
    }

    public void RenderSamples(int offset, int count)
    {
        foreach (var voice in voices)
        {
            voice.Value.RenderSamples(offset, count);
        }
    }

    public float[,] GetStereoBuffer()
    {
        float[,] stereoBuffer = null!;
        foreach (var voice in voices)
        {
            var buffer = voice.Value.GetStereoBuffer();
            var length = buffer.Length / 2;
            if (stereoBuffer == null)
            {
                stereoBuffer = new float[2, length];
            }

            for (var j = 0; j < length; ++j)
            {
                stereoBuffer[0, j] += buffer[0, j];
                stereoBuffer[1, j] += buffer[1, j];
            }
        }

        return stereoBuffer;
    }

    public void TriggerKey(TrackedKey key)
    {
        if (voices.ContainsKey(key.NoteNumber))
        {
            voices[key.NoteNumber].TriggerKey(key);
        }
    }

    public void ReleaseKey(TrackedKey key)
    {
        if (voices.ContainsKey(key.NoteNumber))
        {
            voices[key.NoteNumber].ReleaseKey(key);
        }
    }

    public IVoice? MakeInstanceCopy()
    {
        return null;
    }
}
