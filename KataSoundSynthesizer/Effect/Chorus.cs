#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Oscillators;

namespace KataSoundSynthesizer.Effect;

class Chorus : IEffectComponent
{
    private enum Phase
    {
        Negative180,
        Negative90,
        Zero,
        Positive90,
        Positive180,
    }

    private const float PI2 = (float)(2 * Math.PI);
    private const float FrequencyScale = 5.0f;
    private const int DelayLineMask = 4095;
    private const int DelayLineLength = DelayLineMask + 1;
    private readonly float[] leftDelayLine = new float[DelayLineLength];
    private readonly float[] rightDelayLine = new float[DelayLineLength];
    private Phase phase = Phase.Positive90;
    private float sine;
    private float cosine;
    private float f;
    private int delayLength;
    private int writeIndex;
    private float delay;
    private float frequency;
    private int sampleRate;

    public float Mix { get; set; }
    public float FeedbackLevel { get; set; }
    public float Delay
    {
        get { return delay; }
        set
        {
            delay = value;
            InitializeDefaults();
        }
    }

    public float Depth { get; set; }

    public float Frequency
    {
        get { return frequency; }
        set
        {
            frequency = value;
            InitializeDefaults();
        }
    }

    public int SampleRate
    {
        get { return sampleRate; }
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException("SampleRate", "must be >= 1");
            }

            sampleRate = value;
            InitializeDefaults();
        }
    }

    public Chorus()
    {
        Mix = 0.75f;
        Delay = 0.5f;
        Depth = 0.0003f;
        FeedbackLevel = 0.05f;
        Frequency = 1.0f;
        sampleRate = 1;

        InitializeDefaults();
    }

    private Chorus(Chorus chorus)
    {
        phase = chorus.phase;
        sampleRate = chorus.sampleRate;
        Mix = chorus.Mix;
        FeedbackLevel = chorus.FeedbackLevel;
        Delay = chorus.Delay;
        Depth = chorus.Depth;
        Frequency = chorus.Frequency;

        InitializeDefaults();
    }

    public IEffectComponent MakeInstanceCopy()
    {
        return new Chorus(this);
    }

    private void InitializeDefaults()
    {
        sine = 0;
        cosine = 1;
        f = PI2 * (Frequency * FrequencyScale) / sampleRate;

        delayLength = (int)(Delay * (DelayLineLength / 4));
        writeIndex = 0;
    }

    public void Apply(float[,] input)
    {
        var inputLength = input.Length / 2;
        var readIndex = 0f;
        var delayOutput = 0f;

        for (var i = 0; i < inputLength; ++i)
        {
            IncrementFrequency();

            readIndex = CalculateReadIndex(GetPhaseOutputLeft());
            delayOutput = GetDelayOutput(readIndex, leftDelayLine);
            leftDelayLine[writeIndex] = input[0, i] + delayOutput * FeedbackLevel + 1;

            readIndex = CalculateReadIndex(GetPhaseOutputRight());
            delayOutput = GetDelayOutput(readIndex, rightDelayLine);
            rightDelayLine[writeIndex] = input[1, i] + delayOutput * FeedbackLevel + 1;

            writeIndex = (writeIndex + 1) & DelayLineMask;
        }
    }

    public void Reset()
    {
        Array.Clear(leftDelayLine, 0, leftDelayLine.Length);
        Array.Clear(rightDelayLine, 0, rightDelayLine.Length);
    }

    private void IncrementFrequency()
    {
        sine = sine + f * cosine;
        cosine = cosine - f * sine;
    }

    private float CalculateReadIndex(float output)
    {
        var offset = PowerOfTwoTable.GetPower(output * Depth) * delayLength;
        var readIndex = writeIndex - offset;

        if (readIndex < 0)
        {
            readIndex += DelayLineLength - 1;
            readIndex = Math.Max(readIndex, 0);
        }

        return readIndex;
    }

    private float GetPhaseOutputLeft()
    {
        var phaseOutput = 0.0f;

        switch (phase)
        {
            case Phase.Zero:
            case Phase.Positive90:
            case Phase.Positive180:
                phaseOutput = sine;
                break;

            case Phase.Negative180:
                phaseOutput = -sine;
                break;

            case Phase.Negative90:
                phaseOutput = -cosine;
                break;

            default:
                break;
        }

        return phaseOutput;
    }

    private float GetPhaseOutputRight()
    {
        var phaseOutput = 0.0f;

        switch (phase)
        {
            case Phase.Zero:
                phaseOutput = sine;
                break;

            case Phase.Positive90:
                phaseOutput = cosine;
                break;

            case Phase.Positive180:
                phaseOutput = -sine;
                break;

            case Phase.Negative180:
            case Phase.Negative90:
                phaseOutput = sine;
                break;

            default:
                break;
        }

        return phaseOutput;
    }

    private static float GetDelayOutput(float readIndex, float[] delayLine)
    {
        var n = (int)readIndex;
        var x1 = delayLine[n] - 1;
        var x2 = delayLine[(n + 1) & DelayLineMask] - 1;

        return Lerp(readIndex, x1, x2);
    }

    private static float Lerp(float index, float x1, float x2)
    {
        var fractional = index - (int)index;
        var difference = x1 - x2;
        return x1 - difference * fractional;
    }
}
