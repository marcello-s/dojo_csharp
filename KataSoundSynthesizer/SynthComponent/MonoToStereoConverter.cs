#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion


namespace KataSoundSynthesizer.SynthComponent;

class MonoToStereoConverter(ISynthComponent input)
{
    private float[,] stereoBuffer = new float[2, 1];
    private readonly ISynthComponent input = input;

    public void RenderSamples(int offset, int count)
    {
        if (stereoBuffer != null && stereoBuffer.Length / 2 != count)
        {
            stereoBuffer = new float[2, count];
        }

        var buffer = input.GetMonoBuffer();

        var sampleCount = offset + count;
        if (stereoBuffer != null)
        {
            for (var i = 0; i < sampleCount; ++i)
            {
                stereoBuffer[0, i] += buffer[i];
                stereoBuffer[1, i] += buffer[i];
            }
        }
    }

    public float[,] GetStereoBuffer()
    {
        return stereoBuffer;
    }
}
