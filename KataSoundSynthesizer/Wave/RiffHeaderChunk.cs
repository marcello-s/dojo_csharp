#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using KataSoundSynthesizer.Riff;

namespace KataSoundSynthesizer.Wave;

class RiffHeaderChunk : Chunkbase
{
    // Resource Interchange File Format
    // chunks are marked with a four character code (fourCC)

    public RiffHeaderChunk()
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
                fieldType = ChunkMetadata.ChunkFieldType.AnsiCharacter,
                size =
                    4
                    * ChunkMetadata.ChunkFieldTypeSizeMap[
                        ChunkMetadata.ChunkFieldType.AnsiCharacter
                    ],
            },
        };

        valueMap = CreateValueMapFromField(fields);
    }
}
