#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

interface ISynthComponent
{
    void RenderSamples(int offset, int count);
    int SampleRate { get; set; }
    float[] GetMonoBuffer();

    ISynthComponent MakeInstanceCopy();
}
