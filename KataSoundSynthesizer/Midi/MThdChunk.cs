#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Midi;

class MThdChunk : Chunkbase
{
    public MThdChunk()
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
                name = "header_size",
                fieldType = ChunkMetadata.ChunkFieldType.BigEndianUint32,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.BigEndianUint32
                ],
            },
            new ChunkField
            {
                name = "format",
                fieldType = ChunkMetadata.ChunkFieldType.BigEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.BigEndianUint16
                ],
            },
            new ChunkField
            {
                name = "number_of_tracks",
                fieldType = ChunkMetadata.ChunkFieldType.BigEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.BigEndianUint16
                ],
            },
            new ChunkField
            {
                name = "time_division",
                fieldType = ChunkMetadata.ChunkFieldType.BigEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.BigEndianUint16
                ],
            },
        };

        valueMap = CreateValueMapFromField(fields);
    }
}
