#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataDataCompression;

static class StreamHelper
{
    public static Stream Create(string path)
    {
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public static Stream Create(byte[] buffer)
    {
        return new MemoryStream(buffer);
    }
}
