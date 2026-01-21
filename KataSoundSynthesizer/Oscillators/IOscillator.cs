#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer.Oscillators;

interface IOscillator : ISynthComponent
{
    WaveFormEnum WaveForm { get; set; }
    float Amplitude { get; set; }
    float Frequency { get; set; }
    float Phase { get; set; }
    float DutyCycle { get; set; }
    float CutOff { get; set; }
    float Detune { get; set; }
    float FmLevel { get; set; }
    OctaveMultiplier Multiplier { get; set; }
}
