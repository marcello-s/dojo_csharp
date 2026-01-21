#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Midi;

[TestFixture]
public class FileReaderTest
{
    private const string Ex1FilePath = @"Midi\ex1.mid";
    private const string TheEntertainerFilePath = @"Midi\the_entertainer.mid";
    private const string DebClaiFilePath = @"Midi\deb_clai.mid";
    private const string DrumSampleFilePath = @"Midi\drum_sample.mid";
    private const string DjangoFilePath = @"Midi\django.mid";

    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

    [Test]
    public void Read_WithEx1_ThenFileIsRead()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(Path.Combine(CurrentDirectory, Ex1FilePath), out midiInfo);

        if (tracks != null)
        {
            Assert.That(tracks.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public void Read_WithEntertainer_ThenFileIsRead()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, TheEntertainerFilePath),
            out midiInfo
        );

        if (tracks != null)
        {
            Assert.That(tracks.Count(), Is.EqualTo(2));
        }

        Assert.That(midiInfo.Format, Is.EqualTo(FormatType.MultiTrack));
        Assert.That(midiInfo.NumberOfTracks, Is.EqualTo(2));
        Assert.That(midiInfo.Tempo, Is.EqualTo(TempoType.Metrical));
        Assert.That(midiInfo.PulsesPerQuarterNote, Is.EqualTo(256));
    }

    [Test]
    public void Read_WithDebClai_ThenFileIsRead()
    {
        MidiInfo midiInfo;
        _ = FileReader.Read(Path.Combine(CurrentDirectory, DebClaiFilePath), out midiInfo);
        Assert.That(midiInfo.Format, Is.EqualTo(FormatType.MultiTrack));
        Assert.That(midiInfo.NumberOfTracks, Is.EqualTo(7));
        Assert.That(midiInfo.Tempo, Is.EqualTo(TempoType.Metrical));
        Assert.That(midiInfo.PulsesPerQuarterNote, Is.EqualTo(480));
    }

    [Test]
    public void Read_WithDrumSample_ThenFileIsRead()
    {
        MidiInfo midiInfo;
        _ = FileReader.Read(Path.Combine(CurrentDirectory, DrumSampleFilePath), out midiInfo);
        Assert.That(midiInfo.Format, Is.EqualTo(FormatType.MultiTrack));
        Assert.That(midiInfo.NumberOfTracks, Is.EqualTo(2));
        Assert.That(midiInfo.Tempo, Is.EqualTo(TempoType.Metrical));
        Assert.That(midiInfo.PulsesPerQuarterNote, Is.EqualTo(480));
    }

    [Test, Explicit]
    public void Read_WithEntertainer_ThenPrintTrackInformation()
    {
        MidiInfo midiInfo;
        var tracks = FileReader.Read(
            Path.Combine(CurrentDirectory, TheEntertainerFilePath),
            out midiInfo
        );

        if (tracks != null)
        {
            PrintTrackInformation(tracks);
        }
    }

    private static void PrintTrackInformation(IEnumerable<Track> tracks)
    {
        foreach (var track in tracks)
        {
            PrintEventInformation(track.Events);
        }
    }

    private static void PrintEventInformation(IEnumerable<MidiEventBase> midiEvents)
    {
        foreach (var midiEvent in midiEvents)
        {
            PrintEvent(midiEvent);
        }
    }

    private static void PrintEvent(MidiEventBase midiEvent)
    {
        var metaEvent = midiEvent as MidiMetaEvent;
        if (metaEvent != null)
        {
            if (metaEvent.EventType == MidiMetaEventType.SetTempo)
            {
                Console.WriteLine("microseconds={0}", metaEvent.MicrosecondsPerQuarterNote);
            }
        }

        var noteEvent = midiEvent as MidiEvent;
        if (noteEvent != null)
        {
            if (
                noteEvent.CommandType == MidiEventCommandType.NoteOn
                || noteEvent.CommandType == MidiEventCommandType.NoteOff
            )
            {
                Console.WriteLine(
                    "cmd={0}, ch={1}, key={2}, vel={3},  dt={4}",
                    noteEvent.CommandType,
                    noteEvent.Channel,
                    noteEvent.Param1,
                    noteEvent.Param2,
                    noteEvent.DeltaTime
                );
            }
        }
    }
}
