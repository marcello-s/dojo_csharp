#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

abstract class SynthComponentBase : ISynthComponent
{
    protected float[] monoBuffer = null!;

    public abstract void RenderSamples(int offset, int count);
    public abstract int SampleRate { get; set; }
    public abstract ISynthComponent MakeInstanceCopy();

    protected void InitialiseMonoBuffer(int size)
    {
        if (monoBuffer == null || monoBuffer.Length != size)
        {
            monoBuffer = new float[size];
        }
    }

    protected void SetMonoBuffer(float[] monoBuffer)
    {
        this.monoBuffer = monoBuffer;
    }

    public float[] GetMonoBuffer()
    {
        return monoBuffer;
    }
}
