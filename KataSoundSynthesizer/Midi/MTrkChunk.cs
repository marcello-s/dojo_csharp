#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Midi;

class MTrkChunk : Chunkbase
{
    public byte[]? Data { get; set; }

    public MTrkChunk()
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
                name = "data_size",
                fieldType = ChunkMetadata.ChunkFieldType.BigEndianUint32,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.BigEndianUint32
                ],
            },
        };

        valueMap = CreateValueMapFromField(fields);
    }
}
