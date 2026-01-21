#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

struct TrackedKey
{
    public enum KeyMode
    {
        Trigger,
        Release,
    };

    public KeyMode Mode { get; private set; }
    public long DeltaTimeTicks { get; private set; }
    public int NoteNumber { get; private set; }
    public float Velocity { get; private set; }
    public int Channel { get; private set; }

    public TrackedKey(
        KeyMode mode,
        long deltaTimeTicks,
        int noteNumber,
        float velocity,
        int channel
    )
        : this()
    {
        Mode = mode;
        DeltaTimeTicks = deltaTimeTicks;
        NoteNumber = noteNumber;
        Velocity = velocity;
        Channel = channel;
    }
}
