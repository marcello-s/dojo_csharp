#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class PolyVoice : IVoice
{
    private const int NumberOfVoices = 16;
    private readonly int[] slots = new int[NumberOfVoices];
    private readonly IVoice[] voices = new IVoice[NumberOfVoices];

    public PolyVoice(IVoice voice)
    {
        for (var i = 0; i < NumberOfVoices; ++i)
        {
            var voiceCopy = voice.MakeInstanceCopy();
            if (voiceCopy != null)
            {
                voices[i] = voiceCopy;
            }
        }
    }

    public void RenderSamples(int offset, int count)
    {
        for (var i = 0; i < NumberOfVoices; ++i)
        {
            voices[i].RenderSamples(offset, count);
        }
    }

    public float[,] GetStereoBuffer()
    {
        float[,] stereoBuffer = null!;
        for (var i = 0; i < NumberOfVoices; ++i)
        {
            var buffer = voices[i].GetStereoBuffer();
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

    public float Panning
    {
        get { return voices[0].Panning; }
        set { voices[0].Panning = value; }
    }

    public void TriggerKey(TrackedKey key)
    {
        var slot = GetSlot(slots, 0);
        slots[slot] = key.NoteNumber;
        //System.Console.WriteLine("#slot={0}, note={1}, dt={2}, v={3}, ch={4}",
        //    slot, key.NoteNumber, key.DeltaTimeTicks, key.Velocity, key.Channel);

        voices[slot].TriggerKey(key);
    }

    public void ReleaseKey(TrackedKey key)
    {
        var slot = GetSlot(slots, key.NoteNumber);
        slots[slot] = 0;
        //System.Console.WriteLine("reset #slot={0}, dt={1}", slot, key.DeltaTimeTicks);

        voices[slot].ReleaseKey(key);
    }

    public IVoice? MakeInstanceCopy()
    {
        return null;
    }

    private static int GetSlot(IList<int> slots, int note)
    {
        var index = 0;

        for (var i = 0; i < slots.Count; ++i)
        {
            if (slots[i] != note)
            {
                continue;
            }

            index = i;
            break;
        }

        return index;
    }
}
