#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataDataCompression;

public interface ICompressor
{
    void Compress(Stream input, Stream output);
    void Decompress(Stream input, Stream output);
}
