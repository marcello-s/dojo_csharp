#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Midi;

class MidiInfo
{
    public FormatType Format { get; private set; }
    public ushort NumberOfTracks { get; private set; }
    public TempoType Tempo { get; private set; }
    public ushort PulsesPerQuarterNote { get; private set; }
    public FpsType Fps { get; private set; }
    public ushort SubFrames { get; private set; }

    public MidiInfo(
        FormatType format,
        ushort numberOfTracks,
        TempoType tempo,
        ushort pulsesPerQuaterNote
    )
    {
        Format = format;
        NumberOfTracks = numberOfTracks;
        Tempo = tempo;
        PulsesPerQuarterNote = pulsesPerQuaterNote;
    }

    public MidiInfo(
        FormatType format,
        ushort numberOfTracks,
        TempoType tempo,
        FpsType fps,
        ushort subFrames
    )
    {
        Format = format;
        NumberOfTracks = numberOfTracks;
        Tempo = tempo;
        Fps = fps;
        SubFrames = subFrames;
    }
}

public enum FormatType
{
    SingleTrack = 0,
    MultiTrack = 1,
    IndependendMultiTrack = 3,
}

public enum TempoType
{
    Metrical,
    Timecode,
}

public enum FpsType
{
    Fps24 = 0xE8,
    Fps25 = 0xE7,
    Fps29 = 0xE3,
    Fps30 = 0xE2,
}
