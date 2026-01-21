#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Midi;

static class FileReader
{
    public const string MThd = "MThd";
    public const string MTrk = "MTrk";
    public const ushort MetricalMask = 0x8000; //0x7F;
    public const ushort SubFrameMask = 0x00FF;

    public static IEnumerable<Track>? Read(string filePath, out MidiInfo midiInfo)
    {
        var mthdChunk = new MThdChunk();
        var mtrkChunks = new List<MTrkChunk>();
        midiInfo = null!;

        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            var isChunkRead = ReadChunk(fs, mthdChunk);
            if (isChunkRead)
            {
                var headerMarker = Endianess.ConvertToString((byte[])mthdChunk.ValueMap["marker"]);
                if (headerMarker != MThd)
                {
                    return null; // some error code
                }
            }

            var formatValue = (ushort)mthdChunk.ValueMap["format"];
            var format = (FormatType)Enum.ToObject(typeof(FormatType), formatValue);
            var numberOfTracks = (ushort)mthdChunk.ValueMap["number_of_tracks"];
            var interval = (ushort)mthdChunk.ValueMap["time_division"];
            if ((interval & MetricalMask) == 0)
            {
                midiInfo = new MidiInfo(format, numberOfTracks, TempoType.Metrical, interval);
            }
            else
            {
                var fpsValue = (interval >> 8);
                var fps = (FpsType)Enum.ToObject(typeof(FpsType), fpsValue);
                var subFrames = (ushort)(interval & SubFrameMask);
                midiInfo = new MidiInfo(format, numberOfTracks, TempoType.Timecode, fps, subFrames);
            }

            for (var i = 0; i < numberOfTracks; ++i)
            {
                var mtrkChunk = new MTrkChunk();
                isChunkRead = ReadChunk(fs, mtrkChunk);
                if (!isChunkRead)
                {
                    continue;
                }

                var headerMarker = Endianess.ConvertToString((byte[])mtrkChunk.ValueMap["marker"]);
                if (headerMarker != MTrk)
                {
                    return null; // some error code
                }

                var dataSize = Convert.ToInt32(mtrkChunk.ValueMap["data_size"]);
                mtrkChunk.Data = ReadBuffer(fs, dataSize);
                mtrkChunks.Add(mtrkChunk);
            }
        }

        return DecodeTracks(mtrkChunks);
    }

    private static byte[]? ReadBuffer(Stream s, int length)
    {
        var buffer = new byte[length];
        byte[]? result = null;

        if (s.Read(buffer, 0, length) == length)
        {
            result = buffer;
        }

        return result;
    }

    private static bool ReadChunk(Stream fs, Chunkbase chunk)
    {
        var success = true;

        foreach (var chunkField in chunk.Fields)
        {
            var buffer = ReadBuffer(fs, (int)chunkField.size);
            if (buffer == null)
            {
                success = false;
                break;
            }

            switch (chunkField.fieldType)
            {
                case ChunkMetadata.ChunkFieldType.AnsiCharacter:
                    chunk.ValueMap[chunkField.name] = buffer;
                    break;

                case ChunkMetadata.ChunkFieldType.BigEndianUint32:
                    chunk.ValueMap[chunkField.name] = Endianess.ConvertUintBigToLittle32(buffer);
                    break;

                case ChunkMetadata.ChunkFieldType.BigEndianUint16:
                    chunk.ValueMap[chunkField.name] = Endianess.ConvertUintBigToLittle16(buffer);
                    break;

                default:
                    break;
            }
        }

        return success;
    }

    private static IEnumerable<Track> DecodeTracks(IEnumerable<MTrkChunk> mtrkChunks)
    {
        var tracks = new List<Track>();
        foreach (var trackChunk in mtrkChunks)
        {
            if (trackChunk.Data != null)
            {
                tracks.Add(new Track(MidiEventReader.DecodeBuffer(trackChunk.Data)));
            }
        }

        return tracks;
    }
}
