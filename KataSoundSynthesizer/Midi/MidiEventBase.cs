#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Midi;

class MidiEventBase(uint deltaTime, uint absoluteTime)
{
    public uint DeltaTime { get; private set; } = deltaTime;
    public uint AbsoluteTime { get; private set; } = absoluteTime;
}

class MidiMetaEvent : MidiEventBase
{
    public MidiMetaEventType EventType { get; private set; }
    public string? Text { get; private set; }
    public uint SequenceNumber { get; private set; }
    public uint MicrosecondsPerQuarterNote { get; private set; }
    public byte Numerator { get; private set; }
    public byte Denominator { get; private set; }
    public byte NumberOfTicks { get; private set; }
    public byte NumberOf32ndNotesToTheQuarterNote { get; private set; }
    public KeySharpsFlats SharpsFlats { get; private set; }
    public KeyMajorMinor MajorMinor { get; private set; }
    public byte[]? SequencerData { get; private set; }

    public MidiMetaEvent(uint deltaTime, uint absoluteTime, MidiMetaEventType eventType)
        : base(deltaTime, absoluteTime)
    {
        EventType = eventType;
    }

    public MidiMetaEvent(
        uint deltaTime,
        uint absoluteTime,
        MidiMetaEventType eventType,
        string text
    )
        : base(deltaTime, absoluteTime)
    {
        EventType = eventType;
        Text = text;
    }

    public MidiMetaEvent(
        uint deltaTime,
        uint absoluteTime,
        MidiMetaEventType eventType,
        uint microsecondsPerQuarterNote
    )
        : base(deltaTime, absoluteTime)
    {
        EventType = eventType;
        MicrosecondsPerQuarterNote = microsecondsPerQuarterNote;
    }

    public MidiMetaEvent(
        uint deltaTime,
        uint absoluteTime,
        MidiMetaEventType eventType,
        byte numerator,
        byte denominator,
        byte numberOfTicks,
        byte numberOf32ndNotes
    )
        : base(deltaTime, absoluteTime)
    {
        EventType = eventType;
        Numerator = numerator;
        Denominator = denominator;
        NumberOfTicks = numberOfTicks;
        NumberOf32ndNotesToTheQuarterNote = numberOf32ndNotes;
    }

    public MidiMetaEvent(
        uint deltaTime,
        uint absoluteTime,
        MidiMetaEventType eventType,
        KeySharpsFlats sharpsFlats,
        KeyMajorMinor majorMinor
    )
        : base(deltaTime, absoluteTime)
    {
        EventType = eventType;
        SharpsFlats = sharpsFlats;
        MajorMinor = majorMinor;
    }
}

class MidiEvent(
    uint deltaTime,
    uint absoluteTime,
    MidiEventCommandType commandType,
    byte channel,
    byte param1,
    byte param2
) : MidiEventBase(deltaTime, absoluteTime)
{
    public MidiEventCommandType CommandType { get; private set; } = commandType;
    public byte Channel { get; private set; } = channel;
    public byte Param1 { get; private set; } = param1;
    public byte Param2 { get; private set; } = param2;
}

enum MidiEventType : byte
{
    MetaEvent = 0xff,
    SystemExclusive = 0xf0,
    SystemExclusiveContinuation = 0xf7,
}

enum MidiEventCommandType : byte
{
    NoteOff = 0x80,
    NoteOn = 0x90,
    KeyAfterTouch = 0xa0,
    ControlChange = 0xb0,
    ProgramChange = 0xc0,
    ChannelAfterTouch = 0xd0,
    PitchWheelChange = 0xe0, // value=2000h is normal or no change
}

enum MidiMetaEventType : byte
{
    SetTrackSequenceNo = 0x00,
    Text = 0x01,
    Copyright = 0x02,
    TrackName = 0x03,
    InstrumentName = 0x04,
    Lyric = 0x05,
    Marker = 0x06,
    CuePoint = 0x07,
    TrackEnd = 0x2f,
    SetTempo = 0x51,
    TimeSignature = 0x58,
    KeySignature = 0x59,
    SequencerInformation = 0x7f,
}

enum MidiControlEventType : byte
{
    TimingClock = 0xf8,
    StartSequence = 0xfa,
    ContinueSequence = 0xfb,
    StopSequence = 0xfc,
}

enum KeySharpsFlats : byte
{
    Flats = 1,
    KeyOfC = 0,
    Sharps = 7,
}

enum KeyMajorMinor : byte
{
    Major = 0,
    Minor = 1,
}
