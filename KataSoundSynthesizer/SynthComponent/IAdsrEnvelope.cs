#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

interface IAdsrEnvelope : ISynthComponent
{
    float AttackTime { get; set; }
    float DecayTime { get; set; }
    float SustainLevel { get; set; }
    float ReleaseTime { get; set; }
    float VelocitySensitivity { get; set; }
    TriggerModeEnum TriggerMode { get; set; }
    float SlewTime { get; set; }

    void Trigger(int noteNumber, float velocity);
    void Release(int noteNumber);
}
