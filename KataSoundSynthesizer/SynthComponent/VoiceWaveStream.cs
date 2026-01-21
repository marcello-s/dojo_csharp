#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.SynthComponent;

class VoiceWaveStream : WaveStreamBase
{
    private readonly IVoice voice;
    private readonly int durationInSeconds;
    private long totalSampleCount;
    private readonly TrackedKey[] trackedKeys;
    private readonly int trackedKeysLength;
    private TrackedKey trackedKey;
    private int trackedKeyIndex;
    private float[,]? stereoVoiceSamples;
    private bool abortRendering = false;

    private readonly float[,] vessel = new float[2, 64 * 1024]; // 64kB
    private int vesselSize;
    private int served;

    public VoiceWaveStream(
        WaveFormat waveFormat,
        IVoice voice,
        int durationInSeconds,
        TrackedKey[] trackedKeys
    )
        : base(waveFormat)
    {
        this.voice = voice;
        this.durationInSeconds = durationInSeconds;
        this.trackedKeys = trackedKeys;
        trackedKeysLength = this.trackedKeys.Length - 1;
        trackedKey = this.trackedKeys[trackedKeyIndex];
    }

    protected override int Read(float[]? samples, int offset, int count)
    {
        if (samples == null)
        {
            return 0;
        }

        var sampleRate = WaveFormat.sampleRate;
        var channels = WaveFormat.channels;
        var sampleCount = count / channels;
        var index = 0;

        // buffer rendering
        if (stereoVoiceSamples == null || stereoVoiceSamples.Length / 2 != sampleCount)
        {
            stereoVoiceSamples = new float[2, sampleCount];
        }

        var result = Buffering(stereoVoiceSamples, sampleCount, offset);

        // serve requested sample count
        for (var i = 0; i < sampleCount; ++i)
        {
            samples[index + offset] = stereoVoiceSamples[0, i];
            if (channels == 2)
            {
                samples[index + offset + 1] = stereoVoiceSamples[1, i];
            }

            index += channels;
        }

        totalSampleCount += sampleCount;
        var returnValue = count;
        if (totalSampleCount / sampleRate > durationInSeconds || result == 0)
        {
            returnValue = 0;
        }

        return returnValue;
    }

    private static void CueTrackedKey(IVoice voice, TrackedKey trackedKey)
    {
        if (voice == null)
        {
            return;
        }

        if (trackedKey.Mode == TrackedKey.KeyMode.Trigger)
        {
            voice.TriggerKey(trackedKey);
        }

        if (trackedKey.Mode == TrackedKey.KeyMode.Release)
        {
            voice.ReleaseKey(trackedKey);
        }
    }

    private int Buffering(float[,] buffer, int count, int offset)
    {
        // serve
        while ((vesselSize - served) >= count)
        {
            ArrayCopy(vessel, served, buffer, 0, count);
            served += count;
            return count;
        }

        // fill
        while ((vesselSize - served) < count)
        {
            // render samples
            var data = RenderSamples(offset);
            if (data.Length <= 0)
            {
                return 0;
            }

            ArrayCopy(data, 0, vessel, vesselSize, data.Length / 2);
            vesselSize += data.Length / 2;
        }

        // re-arrange
        vesselSize = vesselSize - served;
        ArrayCopy(vessel, served, vessel, 0, vesselSize);
        served = 0;

        // serve
        ArrayCopy(vessel, served, buffer, 0, count);
        served += count;
        return count;
    }

    private float[,] RenderSamples(int offset)
    {
        if (trackedKeyIndex <= trackedKeysLength)
        {
            CueTrackedKey(voice, trackedKey);
            trackedKeyIndex++;
            trackedKey = trackedKeys[trackedKeyIndex];
        }

        while (trackedKey.DeltaTimeTicks == 0 && trackedKeyIndex <= trackedKeysLength)
        {
            CueTrackedKey(voice, trackedKey);

            if (trackedKeyIndex >= trackedKeysLength)
            {
                continue;
            }

            trackedKeyIndex++;
            trackedKey = trackedKeys[trackedKeyIndex];
        }

        if (abortRendering)
        {
            return new float[0, 0];
        }

        abortRendering = (trackedKeyIndex == trackedKeysLength);
        voice.RenderSamples(offset, (int)trackedKey.DeltaTimeTicks);
        return voice.GetStereoBuffer();
    }

    private static void ArrayCopy(
        float[,] source,
        int sourceIndex,
        float[,] destination,
        int destinationIndex,
        int length
    )
    {
        for (var i = 0; i < length; ++i)
        {
            destination[0, destinationIndex + i] = source[0, sourceIndex + i];
            destination[1, destinationIndex + i] = source[1, sourceIndex + i];
        }
    }
}
