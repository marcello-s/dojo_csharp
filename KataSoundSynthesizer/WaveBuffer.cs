#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataSoundSynthesizer;

[StructLayout(LayoutKind.Explicit, Pack = 2)]
class WaveBuffer
{
    [FieldOffset(0)]
    private int numberOfBytes;

    [FieldOffset(8)]
    private readonly byte[] byteBuffer;

    [FieldOffset(8)]
    private readonly float[]? floatBuffer;

    public WaveBuffer(byte[] byteBuffer)
    {
        this.byteBuffer = byteBuffer;
        numberOfBytes = 0;
    }

    public byte[] ByteBuffer
    {
        get { return byteBuffer; }
    }

    public float[]? FloatBuffer
    {
        get { return floatBuffer; }
    }
}
