#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

interface IFilter : ISynthComponent
{
    float CutOffFrequency { get; set; }
    float Resonance { get; set; }
    float Drive { get; set; }
    FilterType Filter { get; set; }
    float FilterGain { get; set; }
    float Amplitude { get; set; }

    void TriggerKey(int noteNumber);
}
