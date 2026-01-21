#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataSoundSynthesizer.Riff;

struct ChunkField
{
    public string name;
    public ChunkMetadata.ChunkFieldType fieldType;
    public uint size;
}
