#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Effect;

class Echo : IEffectComponent
{
    private const int DelayLineMask = 65535;
    private const int DelayLineLength = DelayLineMask + 1;
    private readonly float[] leftDelayLine = new float[DelayLineLength];
    private readonly float[] rightDelayLine = new float[DelayLineLength];
    private float leftDelayTime;
    private float rightDelayTime;
    private int leftDelayLineOffset;
    private int rightDelayLineOffset;
    private int index;
    private int li,
        ri;
    private int sampleRate;

    public float Mix { get; set; }
    public float FeedbackLevel { get; set; }
    public float LeftDelayTime
    {
        get { return leftDelayTime; }
        set
        {
            if (value < 0.0f)
            {
                throw new ArgumentOutOfRangeException("LeftDelayTime", "must be >= 0");
            }

            leftDelayTime = value;
            InitializeDefaults();
        }
    }

    public float RightDelayTime
    {
        get { return rightDelayTime; }
        set
        {
            if (value < 0.0f)
            {
                throw new ArgumentOutOfRangeException("RightDelayTime", "must be >= 0");
            }

            rightDelayTime = value;
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

    public Echo()
    {
        Mix = 0.75f;
        FeedbackLevel = 0.75f;
        leftDelayTime = 0.5f;
        rightDelayTime = 0.5f;
        sampleRate = 1;

        InitializeDefaults();
    }

    private Echo(Echo echo)
    {
        Mix = echo.Mix;
        FeedbackLevel = echo.FeedbackLevel;
        leftDelayTime = echo.leftDelayTime;
        rightDelayTime = echo.rightDelayTime;
        sampleRate = echo.sampleRate;

        InitializeDefaults();
    }

    public IEffectComponent MakeInstanceCopy()
    {
        return new Echo(this);
    }

    private void InitializeDefaults()
    {
        leftDelayLineOffset = (int)(Math.Min(SampleRate, DelayLineLength) * leftDelayTime);
        rightDelayLineOffset = (int)(Math.Min(SampleRate, DelayLineLength) * rightDelayTime);
        index = 0;
        li = (index + leftDelayLineOffset) & DelayLineMask;
        ri = (index + rightDelayLineOffset) & DelayLineMask;
    }

    public void Apply(float[,] input)
    {
        float feedback;

        li = (index + leftDelayLineOffset) & DelayLineMask;
        ri = (index + rightDelayLineOffset) & DelayLineMask;
        var inputLength = input.Length / 2;

        for (var i = 0; i < inputLength; ++i)
        {
            feedback = leftDelayLine[index] * FeedbackLevel;
            leftDelayLine[li] = input[0, i] * Mix + feedback;
            feedback = rightDelayLine[index] * FeedbackLevel;
            rightDelayLine[ri] = input[1, i] * Mix + feedback;

            input[0, i] += leftDelayLine[index];
            input[1, i] += rightDelayLine[index];

            li = (li + 1) & DelayLineMask;
            ri = (ri + 1) & DelayLineMask;
            index = (index + 1) & DelayLineMask;
        }
    }

    public void Reset()
    {
        Array.Clear(leftDelayLine, 0, leftDelayLine.Length);
        Array.Clear(rightDelayLine, 0, rightDelayLine.Length);
    }
}
