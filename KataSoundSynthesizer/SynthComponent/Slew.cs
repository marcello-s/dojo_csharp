#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class Slew : SynthComponentBase, IAdsrEnvelope
{
    private const int NotesPerOctave = 12;
    private int previousNoteNumber;
    private float slewTime;
    private Direction direction;
    private float target;
    private float a0,
        b1;
    private float output;

    public override int SampleRate { get; set; }
    public float AttackTime { get; set; }
    public float DecayTime { get; set; }
    public float SustainLevel { get; set; }
    public float ReleaseTime { get; set; }
    public float VelocitySensitivity { get; set; }
    public TriggerModeEnum TriggerMode { get; set; }

    public float SlewTime
    {
        get { return slewTime; }
        set
        {
            if (value < 0f)
            {
                throw new ArgumentOutOfRangeException("SlewTime", "must be >= 0");
            }

            slewTime = value;
        }
    }

    public Slew()
    {
        slewTime = 0.25f;
        direction = Direction.Up;
        target = 0f;
    }

    private Slew(IAdsrEnvelope adsrEnvelope)
    {
        SlewTime = adsrEnvelope.SlewTime;
        direction = Direction.Up;
        target = 0f;
    }

    public override ISynthComponent MakeInstanceCopy()
    {
        return new Slew(this);
    }

    public void Trigger(int noteNumber, float velocity)
    {
        var s = slewTime * SampleRate + 1;
        var x = (float)Math.Exp(-1 / s);

        a0 = x - 1;
        b1 = x;

        output = (float)(previousNoteNumber - noteNumber) / NotesPerOctave;

        if (output < 0)
        {
            direction = Direction.Up;
            target = 0.01f;
        }
        else
        {
            direction = Direction.Down;
            target = -0.01f;
        }

        previousNoteNumber = noteNumber;
    }

    public void Release(int noteNumber) { }

    public override void RenderSamples(int offset, int count)
    {
        InitialiseMonoBuffer(count);

        var o = 0f;

        var buffer = GetMonoBuffer();
        var sampleCount = offset + count;
        for (var i = offset; i < sampleCount; ++i)
        {
            o = 0f;

            if (direction == Direction.Up)
            {
                if (output < target)
                {
                    o = a0 * target + b1 * output;
                    output = o;
                }
            }
            else
            {
                if (output > target)
                {
                    o = a0 * target + b1 * output;
                    output = o;
                }
            }

            monoBuffer[i] += o;
        }
    }
}

enum Direction
{
    Up,
    Down,
}
