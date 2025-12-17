#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.InteropServices;

namespace KataCompiler.Vm;

[StructLayout(LayoutKind.Explicit)]
struct PrimitivePacker
{
    [FieldOffset(0)]
    public byte b0;

    [FieldOffset(1)]
    public byte b1;

    [FieldOffset(2)]
    public byte b2;

    [FieldOffset(3)]
    public byte b3;

    [FieldOffset(4)]
    public byte b4;

    [FieldOffset(5)]
    public byte b5;

    [FieldOffset(6)]
    public byte b6;

    [FieldOffset(7)]
    public byte b7;

    [FieldOffset(0)]
    public int i0;

    [FieldOffset(4)]
    public int i1;

    [FieldOffset(0)]
    public double d0;

    [FieldOffset(0)]
    public uint ui0;

    [FieldOffset(4)]
    public uint ui1;

    [FieldOffset(0)]
    public bool bl0;
}
