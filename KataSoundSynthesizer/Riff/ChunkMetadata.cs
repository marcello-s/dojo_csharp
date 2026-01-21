#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Riff;

public static class ChunkMetadata
{
    public enum ChunkFieldType
    {
        AnsiCharacter,
        BigEndianUint16,
        BigEndianUint32,
        LittleEndianUint16,
        LittleEndianUint32,
    }

    public static readonly IDictionary<ChunkFieldType, uint> ChunkFieldTypeSizeMap = new Dictionary<
        ChunkFieldType,
        uint
    >
    {
        { ChunkFieldType.AnsiCharacter, sizeof(byte) },
        { ChunkFieldType.BigEndianUint16, sizeof(short) },
        { ChunkFieldType.BigEndianUint32, sizeof(int) },
        { ChunkFieldType.LittleEndianUint16, sizeof(short) },
        { ChunkFieldType.LittleEndianUint32, sizeof(int) },
    };
}
