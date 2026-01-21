#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Wave;

class FormatChunk : Chunkbase
{
    public FormatChunk()
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
            new ChunkField
            {
                name = "format",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint16
                ],
            },
            new ChunkField
            {
                name = "channels",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint16
                ],
            },
            new ChunkField
            {
                name = "sample_rate",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint32,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint32
                ],
            },
            new ChunkField
            {
                name = "byte_rate",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint32,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint32
                ],
            },
            new ChunkField
            {
                name = "block_align",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint16
                ],
            },
            new ChunkField
            {
                name = "bits_per_sample",
                fieldType = ChunkMetadata.ChunkFieldType.LittleEndianUint16,
                size = ChunkMetadata.ChunkFieldTypeSizeMap[
                    ChunkMetadata.ChunkFieldType.LittleEndianUint16
                ],
            },
        };

        valueMap = CreateValueMapFromField(fields);
    }
}
