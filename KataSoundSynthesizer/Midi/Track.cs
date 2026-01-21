#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Midi;

class Track
{
    public IEnumerable<MidiEventBase> Events { get; private set; }

    public Track(IEnumerable<MidiEventBase> events)
    {
        if (events == null)
        {
            throw new ArgumentNullException("events");
        }

        Events = events;
    }
}
