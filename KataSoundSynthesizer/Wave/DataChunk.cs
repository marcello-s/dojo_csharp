#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Wave;

class DataChunk : Chunkbase
{
    public byte[]? Data { get; set; }

    public DataChunk()
    {
        fields = new List<ChunkField>
        {
            new ChunkField
            {
                name = "marker",
                fieldType = ChunkMetadata.ChunkFieldType.AnsiCharacter,
                size =
                    4
                    * ChunkMetadata.ChunkFieldTypeSizeMap[
                        ChunkMetadata.ChunkFieldType.AnsiCharacter
                    ],
            },
            new ChunkField
            {
                name = "size",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint32,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint32
                ],
            },
        };

        valueMap = CreateValueMapFromField(fields);
    }
}
