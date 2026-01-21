#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer.Wave;

class WaveVoice(float[] buffer, bool isReversed = false, bool isRepeating = false) : IVoice
{
    private readonly float[] buffer =
        isReversed == true ? WaveDataConverter.ReverseFloatBuffer(buffer) : buffer;
    private readonly bool isRepeating = isRepeating;
    private int bufferIndex;
    private bool isRunToEnd;
    private float volume;
    private float[,] samples = new float[2, 1];

    public float Panning { get; set; }

    public void RenderSamples(int offset, int count)
    {
        if (samples == null || samples.Length != count)
        {
            samples = new float[2, count];
        }

        for (var i = 0; i < count; ++i)
        {
            if (bufferIndex >= buffer.Length - 1)
            {
                bufferIndex = 0;
                isRunToEnd = !isRepeating;
            }

            samples[0, i] = volume * buffer[bufferIndex];
            samples[1, i] = volume * buffer[bufferIndex + 1];

            if (!isRunToEnd)
            {
                bufferIndex += 2;
            }
        }
    }

    public float[]? GetMonoBuffer()
    {
        return null;
    }

    public float[,] GetStereoBuffer()
    {
        return samples;
    }

    public void TriggerKey(TrackedKey key)
    {
        volume = key.Velocity;
        bufferIndex = 0;
        isRunToEnd = false;
    }

    public void ReleaseKey(TrackedKey key)
    {
        bufferIndex = buffer.Length;
    }

    public IVoice? MakeInstanceCopy()
    {
        throw new NotImplementedException();
    }
}
