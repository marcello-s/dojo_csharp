#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Wave;

class FileReader
{
    public const string Riff = "RIFF";
    public const string Wave = "WAVE";
    public const string Fmt = "fmt "; // note the extra space
    public const string Data = "data";
    public const ushort FormatPCM = 1;

    public static WaveData? Read(string filePath)
    {
        var riffHeaderChunk = new RiffHeaderChunk();
        var formatChunk = new FormatChunk();
        var dataChunk = new DataChunk();

        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            var isChunkRead = ReadChunk(fs, riffHeaderChunk);
            if (isChunkRead)
            {
                var headerMarker = Endianess.ConvertToString(
                    (byte[])riffHeaderChunk.ValueMap["marker"]
                );
                if (headerMarker != Riff)
                {
                    return null; // return an error
                }

                var chunkSize = riffHeaderChunk.ValueMap["size"];
                var format = Endianess.ConvertToString((byte[])riffHeaderChunk.ValueMap["format"]);
                if (format != Wave)
                {
                    return null; // return an error
                }
            }

            isChunkRead = ReadChunk(fs, formatChunk);
            if (isChunkRead)
            {
                var formatMarker = Endianess.ConvertToString(
                    (byte[])formatChunk.ValueMap["marker"]
                );
                if (formatMarker != Fmt)
                {
                    return null; // return an error
                }

                var format = formatChunk.ValueMap["format"];
                if ((ushort)format != FormatPCM)
                {
                    return null; // only PCM supported
                }
            }

            isChunkRead = ReadChunk(fs, dataChunk);
            if (isChunkRead)
            {
                var dataMarker = Endianess.ConvertToString((byte[])dataChunk.ValueMap["marker"]);
                if (dataMarker != Data)
                {
                    return null; // return an error
                }

                var size = Convert.ToInt32(dataChunk.ValueMap["size"]);
                var data = ReadBuffer(fs, size);

                if (data != null)
                {
                    dataChunk.Data = data;
                }
                else
                {
                    return null; // return an error
                }
            }
        }

        return CreateWaveData(formatChunk, dataChunk);
    }

    private static bool ReadChunk(Stream s, Chunkbase chunk)
    {
        var success = true;

        foreach (var chunkField in chunk.Fields)
        {
            var buffer = ReadBuffer(s, (int)chunkField.size);
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

                case ChunkMetadata.ChunkFieldType.LittleEndianUint16:
                    chunk.ValueMap[chunkField.name] = Endianess.ConvertUintLittleToBig16(buffer);
                    break;

                case ChunkMetadata.ChunkFieldType.LittleEndianUint32:
                    chunk.ValueMap[chunkField.name] = Endianess.ConvertUintLittleToBig32(buffer);
                    break;

                default:
                    break;
            }
        }

        return success;
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

    private static WaveData CreateWaveData(Chunkbase formatChunk, DataChunk dataChunk)
    {
        var channels = Convert.ToInt16(formatChunk.ValueMap["channels"]);
        var sampleRate = Convert.ToInt32(formatChunk.ValueMap["sample_rate"]);
        var bitsPerChannel = Convert.ToInt16(formatChunk.ValueMap["bits_per_sample"]);

        return new WaveData(channels, sampleRate, bitsPerChannel, dataChunk.Data);
    }
}
