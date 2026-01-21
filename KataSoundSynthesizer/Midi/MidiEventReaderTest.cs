#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataSoundSynthesizer.Midi;

[TestFixture]
public class MidiEventReaderTest
{
    [Test]
    public void ReadVariableLengthEncodedValue_WhenZero_ThenReturnValue()
    {
        var buffer = new byte[] { 0 };
        uint value;
        var read = MidiEventReader.ReadVariableLengthEncodedValue(0, buffer, out value);
        Assert.That(read, Is.EqualTo(1));
        Assert.That(value, Is.EqualTo(0));
    }

    [Test]
    public void ReadVariableLengthEncodedValue_When1Byte_ThenReturnValue()
    {
        var buffer = new byte[] { 0x40 };
        uint value;
        var read = MidiEventReader.ReadVariableLengthEncodedValue(0, buffer, out value);
        Assert.That(read, Is.EqualTo(1));
        Assert.That(value, Is.EqualTo(0x40));
    }

    [Test]
    public void ReadVariableLengthEncodedValue_When1ByteAgain_ThenReturnValue()
    {
        var buffer = new byte[] { 0x7f };
        uint value;
        var read = MidiEventReader.ReadVariableLengthEncodedValue(0, buffer, out value);
        Assert.That(read, Is.EqualTo(1));
        Assert.That(value, Is.EqualTo(0x7f));
    }

    [Test]
    public void ReadVariableLengthEncodedValue_When2Bytes_ThenReturnValue()
    {
        var buffer = new byte[] { 0x81, 0x00 };
        uint value;
        var read = MidiEventReader.ReadVariableLengthEncodedValue(0, buffer, out value);
        Assert.That(read, Is.EqualTo(2));
        Assert.That(value, Is.EqualTo(0x80));
    }

    [Test]
    public void ReadVariableLengthEncodedValue_When2BytesAgain_ThenReturnValue()
    {
        var buffer = new byte[] { 0xc0, 0x00 };
        uint value;
        var read = MidiEventReader.ReadVariableLengthEncodedValue(0, buffer, out value);
        Assert.That(read, Is.EqualTo(2));
        Assert.That(value, Is.EqualTo(0x2000));
    }

    [Test]
    public void DecodeBuffer_WhenMetaEvent_ThenReturnEvents()
    {
        var buffer = new byte[]
        {
            0x00,
            0xff,
            0x03,
            0x07,
            0x54,
            0x72,
            0x61,
            0x63,
            0x6b,
            0x20,
            0x31,
        };
        var events = MidiEventReader.DecodeBuffer(buffer);
        Assert.That(events.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DecodeBuffer_WhenMidiEvent_ThenReturnEvents()
    {
        var buffer = new byte[] { 0x00, 0x90, 0x30, 0x46 };
        var events = MidiEventReader.DecodeBuffer(buffer);
        Assert.That(events.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DecodeBuffer_WhenMetaEventSetTempo_ThenReturnEvents()
    {
        var buffer = new byte[] { 0x00, 0xff, 0x51, 0x03, 0x07, 0xa1, 0x20 };
        var events = MidiEventReader.DecodeBuffer(buffer);
        Assert.That(events.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DecodeBuffer_WhenMetaEventTimeSignature_ThenReturnEvents()
    {
        var buffer = new byte[] { 0x00, 0xff, 0x58, 0x02, 0x04, 0x02 };
        var events = MidiEventReader.DecodeBuffer(buffer);
        Assert.That(events.Count(), Is.EqualTo(1));
    }

    [Test]
    public void DecodeBuffer_WhenMetaEventTrackEnd_ThenReturnEvents()
    {
        var buffer = new byte[] { 0x00, 0xff, 0x2f, 0x00 };
        var events = MidiEventReader.DecodeBuffer(buffer);
        Assert.That(events.Count(), Is.EqualTo(1));
    }
}
