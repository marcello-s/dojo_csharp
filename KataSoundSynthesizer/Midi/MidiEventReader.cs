#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Midi;

class MidiEventReader
{
    private const byte SetMsb = 0x80;
    private const byte CommandMask = 0xf0;
    private const byte ChannelMask = 0x0f;
    private static readonly MidiEventCommandType[] NeedParam2 = new[]
    {
        MidiEventCommandType.NoteOff,
        MidiEventCommandType.NoteOn,
        MidiEventCommandType.KeyAfterTouch,
        MidiEventCommandType.ControlChange,
        MidiEventCommandType.PitchWheelChange,
    };

    public static IEnumerable<MidiEventBase> DecodeBuffer(byte[] buffer)
    {
        var midiEvents = new List<MidiEventBase>();
        var index = 0;
        uint absoluteTime = 0;
        byte previousCommand = 0;

        while (index < buffer.Length)
        {
            // read delta time
            uint deltaTime = 0;
            index = ReadVariableLengthEncodedValue(index, buffer, out deltaTime);
            absoluteTime += deltaTime;

            var command = buffer[index++];
            var eventType = (MidiEventType)Enum.ToObject(typeof(MidiEventType), command);
            MidiEventBase? metaEvent = null;

            switch (eventType)
            {
                case MidiEventType.MetaEvent:
                    index = ReadMetaEvent(buffer, index, deltaTime, absoluteTime, out metaEvent);
                    break;

                case MidiEventType.SystemExclusive:
                    break;

                case MidiEventType.SystemExclusiveContinuation:
                    break;

                default:
                    index = ReadEvent(
                        buffer,
                        index,
                        command,
                        deltaTime,
                        absoluteTime,
                        ref previousCommand,
                        out metaEvent
                    );
                    break;
            }

            if (metaEvent != null)
            {
                midiEvents.Add(metaEvent);
            }
        }

        return midiEvents;
    }

    private static int ReadMetaEvent(
        byte[] buffer,
        int index,
        uint deltaTime,
        uint absoluteTime,
        out MidiEventBase? metaEvent
    )
    {
        // event format:
        // - event type= 0xff
        // - command (byte)
        // - length (byte)
        // - data

        var command = buffer[index++];
        var length = buffer[index++];

        var eventType = (MidiMetaEventType)Enum.ToObject(typeof(MidiMetaEventType), command);

        // a text event 01-07
        if (eventType >= MidiMetaEventType.Text && eventType <= MidiMetaEventType.CuePoint)
        {
            var textBuffer = new byte[length];
            for (var i = 0; i < length; ++i)
            {
                textBuffer[i] = buffer[index++];
            }

            var text = Endianess.ConvertToString(textBuffer);
            metaEvent = new MidiMetaEvent(deltaTime, absoluteTime, eventType, text);
        }
        else if (eventType == MidiMetaEventType.SetTempo)
        {
            uint microseconds = 0;
            for (var i = 0; i < length - 1; ++i)
            {
                microseconds += buffer[index++];
                microseconds <<= 8;
            }

            microseconds += buffer[index++];

            metaEvent = new MidiMetaEvent(deltaTime, absoluteTime, eventType, microseconds);
        }
        else if (eventType == MidiMetaEventType.TimeSignature)
        {
            var time = new byte[4];
            for (var i = 0; i < length; ++i)
            {
                time[i] = buffer[index++];
            }

            metaEvent = new MidiMetaEvent(
                deltaTime,
                absoluteTime,
                eventType,
                time[0],
                time[1],
                time[2],
                time[3]
            );
        }
        else if (eventType == MidiMetaEventType.KeySignature)
        {
            var keySignature = new byte[2];
            for (var i = 0; i < length; ++i)
            {
                keySignature[i] = buffer[index++];
            }

            var sharpsFlats = (KeySharpsFlats)
                Enum.ToObject(typeof(KeySharpsFlats), keySignature[0]);
            var majorMinor = (KeyMajorMinor)Enum.ToObject(typeof(KeyMajorMinor), keySignature[1]);

            metaEvent = new MidiMetaEvent(
                deltaTime,
                absoluteTime,
                eventType,
                sharpsFlats,
                majorMinor
            );
        }
        else if (eventType == MidiMetaEventType.TrackEnd)
        {
            metaEvent = new MidiMetaEvent(deltaTime, absoluteTime, eventType);
        }
        else
        {
            metaEvent = null;
        }

        return index;
    }

    private static int ReadEvent(
        byte[] buffer,
        int index,
        byte command,
        uint deltaTime,
        uint absoluteTime,
        ref byte previousCommand,
        out MidiEventBase midiEvent
    )
    {
        // event format:
        // - command|channel= 0x90..
        // - param1 (byte)
        // - param2 (byte) [optional]

        // if the command is not a command, we have advanced to param1 already and we
        // take the command/channel from the previous event

        byte param1;
        if (!IsCommand(command))
        {
            param1 = command;
            command = previousCommand;
        }
        else
        {
            previousCommand = command;
            param1 = buffer[index++];
        }

        var channel = (byte)(command & ChannelMask);
        channel++; // channels are 1-based
        command &= CommandMask;

        var commandType = (MidiEventCommandType)
            Enum.ToObject(typeof(MidiEventCommandType), command);

        //var param1 = buffer[index++];
        byte param2 = 0;
        if (NeedParam2.Contains(commandType))
        {
            param2 = buffer[index++];
        }

        midiEvent = new MidiEvent(deltaTime, absoluteTime, commandType, channel, param1, param2);
        return index;
    }

    public static int ReadVariableLengthEncodedValue(int index, byte[] buffer, out uint value)
    {
        const byte SetMsb = 0x80;
        const byte ClearMsb = 0x7f;
        const byte ShiftFactor = 7; // 2^7=128

        value = buffer[index];

        if ((value & SetMsb) == SetMsb)
        {
            value &= ClearMsb;

            uint fetch = 0;

            do
            {
                index++;
                fetch = buffer[index];
                value <<= ShiftFactor;
                value |= fetch & ClearMsb;
            } while ((fetch & SetMsb) == SetMsb);
        }

        return index + 1;
    }

    public static bool IsCommand(byte command)
    {
        return (command & SetMsb) == SetMsb;
    }
}
