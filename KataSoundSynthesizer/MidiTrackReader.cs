#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Midi;
using KataSoundSynthesizer.SynthComponent;

namespace KataSoundSynthesizer;

class MidiTrackReader(int sampleRate, int octaveScale)
{
    // This class is multiplexing midi events from all tracks, always
    // choosing the event with the smallest delay tick

    private const uint MicroSecondsPerQuarterNoteDefault = 240000;
    private const int TonesPerOctave = 12;

    private uint mspqn = MicroSecondsPerQuarterNoteDefault;
    private readonly int sampleRate = sampleRate;
    private readonly int octaveScale = octaveScale;

    public IEnumerable<TrackedKey> Read(IEnumerable<Track> tracks)
    {
        var trackedKeys = new List<TrackedKey>();
        var trackReaderInfo = new List<TrackReaderInfo>(tracks.Count());
        var key = new TrackedKey(TrackedKey.KeyMode.Release, 0, 0, 0f, 0);

        // initialize track reader info
        for (var i = 0; i < tracks.Count(); ++i)
        {
            var track = tracks.ElementAt(i);
            var tri = new TrackReaderInfo
            {
                NumberOfEvents = track.Events.Count(),
                CurrentEvent = track.Events.ElementAt(0),
                Track = track,
                TrackNumber = i,
            };
            IncrementEventIndex(ref tri);
            trackReaderInfo.Add(tri);
        }

        // while not all track reader info read to max
        while (!AreAllTrackEventsRead(trackReaderInfo))
        {
            // choose an event
            var chosenTrackReaderInfo = 0;
            for (var i = 0; i < trackReaderInfo.Count(); ++i)
            {
                var ev = trackReaderInfo[i].CurrentEvent;
                if (ev == null)
                {
                    continue;
                }

                if (ev.DeltaTime < trackReaderInfo[chosenTrackReaderInfo].CurrentEvent?.DeltaTime)
                {
                    chosenTrackReaderInfo = i;
                }
            }

            // do something
            var chosenTri = trackReaderInfo.ElementAt(chosenTrackReaderInfo);
            if (chosenTri.CurrentEvent != null && ProcessEvent(chosenTri.CurrentEvent, out key))
            {
                trackedKeys.Add(key);
            }

            // update track reader info
            UpdateEvent(ref chosenTri);
            trackReaderInfo[chosenTrackReaderInfo] = chosenTri;

            // remove from list when event is null
            if (chosenTri.CurrentEvent == null)
            {
                trackReaderInfo.Remove(chosenTri);
            }
        }

        return trackedKeys;
    }

    private static bool IncrementEventIndex(ref TrackReaderInfo tri)
    {
        var updated = false;
        if (tri.EventIndex < tri.NumberOfEvents)
        {
            tri.EventIndex++;
            updated = true;
        }

        return updated;
    }

    private static void UpdateEvent(ref TrackReaderInfo tri)
    {
        tri.CurrentEvent = IncrementEventIndex(ref tri)
            ? tri.Track.Events.ElementAt(tri.EventIndex - 1)
            : null;
    }

    private static bool AreAllTrackEventsRead(IEnumerable<TrackReaderInfo> tri)
    {
        return tri.All(info => info.EventIndex >= info.NumberOfEvents);
    }

    private struct TrackReaderInfo
    {
        public int NumberOfEvents { get; set; }
        public int EventIndex { get; set; }
        public MidiEventBase? CurrentEvent { get; set; }
        public Track Track { get; set; }
        public int TrackNumber { get; set; }
    }

    private bool ProcessEvent(MidiEventBase ev, out TrackedKey key)
    {
        key = new TrackedKey(TrackedKey.KeyMode.Trigger, 0, 0, 0f, 0);

        if (ev is MidiMetaEvent)
        {
            var metaEvent = ev as MidiMetaEvent;
            if (metaEvent == null)
            {
                return false;
            }

            if (metaEvent.EventType == MidiMetaEventType.SetTempo)
            {
                mspqn = metaEvent.MicrosecondsPerQuarterNote;
            }

            if (metaEvent.EventType == MidiMetaEventType.TimeSignature)
            {
                Console.WriteLine(
                    "time signature - num:{0} den:{1} ticks:{2} qn:{3}",
                    metaEvent.Numerator,
                    metaEvent.Denominator,
                    metaEvent.NumberOfTicks,
                    metaEvent.NumberOf32ndNotesToTheQuarterNote
                );
                var denominator = Math.Pow(2, metaEvent.Denominator);
                var ticks = (24 / 0.25) * (metaEvent.Numerator / denominator);
                var metronomeTick = ticks / metaEvent.NumberOfTicks;
                Console.WriteLine(
                    "{0}/{1} - metronome every {2} of 1/{3}",
                    metaEvent.Numerator,
                    denominator,
                    metronomeTick,
                    metaEvent.NumberOf32ndNotesToTheQuarterNote
                );
            }

            return false;
        }

        if (ev is MidiEvent)
        {
            var midiEvent = ev as MidiEvent;
            if (midiEvent == null)
            {
                return false;
            }

            key = CreateTrackedKeyFromNoteEvent(midiEvent);

            return true;
        }

        return false;
    }

    private TrackedKey CreateTrackedKeyFromNoteEvent(MidiEvent me)
    {
        if (me.CommandType == MidiEventCommandType.NoteOn)
        {
            // a velocity of 0 will create a release event
            if (me.Param2 != 0)
            {
                return new TrackedKey(
                    TrackedKey.KeyMode.Trigger,
                    TimeTicks(mspqn, sampleRate, (int)me.DeltaTime),
                    Transpose(me.Param1, octaveScale),
                    me.Param2 / 100.0f,
                    me.Channel
                );
            }
            else
            {
                return new TrackedKey(
                    TrackedKey.KeyMode.Release,
                    TimeTicks(mspqn, sampleRate, (int)me.DeltaTime),
                    Transpose(me.Param1, octaveScale),
                    me.Param2 / 100.0f,
                    me.Channel
                );
            }
        }

        if (me.CommandType == MidiEventCommandType.NoteOff)
        {
            return new TrackedKey(
                TrackedKey.KeyMode.Release,
                TimeTicks(mspqn, sampleRate, (int)me.DeltaTime),
                Transpose(me.Param1, octaveScale),
                me.Param2 / 100.0f,
                me.Channel
            );
        }

        return new TrackedKey(TrackedKey.KeyMode.Release, 0, 0, 0f, 0);
    }

    private static long TimeTicks(uint mspqn, int sampleRate, int dt)
    {
        return mspqn / sampleRate * dt;
    }

    private static int Transpose(int key, int octaveScale)
    {
        return Math.Max(0, key + octaveScale * TonesPerOctave);
    }
}
