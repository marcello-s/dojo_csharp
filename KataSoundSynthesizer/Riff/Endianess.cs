#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataSoundSynthesizer.Riff;

static class Endianess
{
    public static uint ConvertUintBigToLittle32(byte[] buffer)
    {
        if (buffer.Length != 4)
        {
            throw new ArgumentOutOfRangeException("buffer", buffer.Length, "length not equal to 4");
        }

        return ((uint)buffer[0] << 24)
            | ((uint)buffer[1] << 16)
            | ((uint)buffer[2] << 8)
            | buffer[3];
    }

    public static ushort ConvertUintBigToLittle16(byte[] buffer)
    {
        if (buffer.Length != 2)
        {
            throw new ArgumentOutOfRangeException("buffer", buffer.Length, "length not equal to 2");
        }

        return Convert.ToUInt16((buffer[0] << 8) | buffer[1]);
    }

    public static uint ConvertUintLittleToBig32(byte[] buffer)
    {
        if (buffer.Length != 4)
        {
            throw new ArgumentOutOfRangeException("buffer", buffer.Length, "length not equal to 4");
        }

        return BitConverter.ToUInt32(buffer, 0);
    }

    public static ushort ConvertUintLittleToBig16(byte[] buffer)
    {
        if (buffer.Length != 2)
        {
            throw new ArgumentOutOfRangeException("buffer", buffer.Length, "length not equal to 2");
        }

        return Convert.ToUInt16((buffer[1] << 8) | buffer[0]);
    }

    public static string ConvertToString(byte[] buffer)
    {
        return Encoding.Default.GetString(buffer);
    }
}
