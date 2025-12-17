#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCompiler.Vm;

[TestFixture]
public class PrimitivePackerTests
{
    [Test, Ignore("because")]
    public void Byte_Packer()
    {
        Console.WriteLine(
            "sizeof int:{0}, float:{1}, double:{2}, bool:{3}, char:{4}",
            sizeof(int),
            sizeof(float),
            sizeof(double),
            sizeof(bool),
            sizeof(char)
        );

        var bytes = BitConverter.GetBytes(22.0 / 7.0);
        foreach (var b in bytes)
        {
            Console.WriteLine(b);
        }

        var d = BitConverter.ToDouble(bytes, 0);
        Console.WriteLine(d);

        var i0 = BitConverter.ToInt32(bytes, 0);
        var i1 = BitConverter.ToInt32(bytes, 4);
        Console.WriteLine("{0}, {1}", i0, i1);

        var ba0 = BitConverter.GetBytes(i0);
        var ba1 = BitConverter.GetBytes(i1);
        var ba = new byte[ba0.Length + ba1.Length];

        for (var i = 0; i < ba0.Length; ++i)
        {
            ba[i] = ba0[i];
        }

        for (var i = 0; i < ba1.Length; ++i)
        {
            ba[ba0.Length + i] = ba1[i];
        }

        var d1 = BitConverter.ToDouble(ba, 0);
        Console.WriteLine(d1);
    }

    [Test]
    public void PrimitivePacker_double_byte()
    {
        var p = new PrimitivePacker();
        p.d0 = 22.0 / 7.0;

        // 73 146 36 73 146 36 9 64
        Assert.That(p.b0, Is.EqualTo(73));
        Assert.That(p.b1, Is.EqualTo(146));
        Assert.That(p.b2, Is.EqualTo(36));
        Assert.That(p.b3, Is.EqualTo(73));
        Assert.That(p.b4, Is.EqualTo(146));
        Assert.That(p.b5, Is.EqualTo(36));
        Assert.That(p.b6, Is.EqualTo(9));
        Assert.That(p.b7, Is.EqualTo(64));
    }

    [Test]
    public void PrimitivePacker_uint_double()
    {
        // 1227133513 1074341010
        var p = new PrimitivePacker();
        p.ui0 = 1227133513;
        p.ui1 = 1074341010;
        Assert.That(p.d0, Is.EqualTo(3.1428571428571428));
    }

    [Test]
    public void PrimitivePacker_int_uint()
    {
        // -33
        var p = new PrimitivePacker();
        p.i0 = -33;
        //Console.WriteLine("0x{0:x}", p.ui0);
        Assert.That(p.ui0, Is.EqualTo(4294967263));
    }
}
