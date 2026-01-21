#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class VoiceWaveSample(string waveSampleFilepath, int noteNumber)
{
    public string WaveSampleFilepath { get; private set; } = waveSampleFilepath;
    public int NoteNumber { get; private set; } = noteNumber;
}
