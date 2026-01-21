#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

interface IVoice
{
    void RenderSamples(int offset, int count);
    float[,] GetStereoBuffer();
    float Panning { get; set; }

    void TriggerKey(TrackedKey key);
    void ReleaseKey(TrackedKey key);

    IVoice? MakeInstanceCopy();
}
