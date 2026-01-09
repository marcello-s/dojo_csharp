#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;
using NUnit.Framework;

namespace KataCompiler.Vm;

[TestFixture]
public class CpuTests
{
    [Test]
    public void Run_LoadAConst()
    {
        // load A, const
        // halt

        uint[] code = { (uint)Mnem.LoadAConst, 22, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(22));
    }

    [Test]
    public void Run_LoadD0Const()
    {
        // load D0, const
        // halt

        uint[] code = { (uint)Mnem.LoadD0Const, 1227133513, 1074341010, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.D0, Is.EqualTo(3.1428571428571428));
    }

    [Test]
    public void Run_LoadD1Const()
    {
        // load D1, const
        // halt

        uint[] code = { (uint)Mnem.LoadD1Const, 1227133513, 1074341010, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.D1, Is.EqualTo(3.1428571428571428));
    }

    [Test]
    public void Run_LoadSPConst()
    {
        // load SP, const
        // halt

        uint[] code = { (uint)Mnem.LoadSpConst, 9, (uint)Mnem.Halt };
        var data = new uint[10];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.SP, Is.EqualTo(9));
    }

    [Test]
    public void Run_MoveD0Data()
    {
        // load D0, const
        // move #data, D0
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadD0Const,
            1227133513,
            1074341010,
            (uint)Mnem.MoveDataD0,
            0,
            (uint)Mnem.Halt,
        };
        var data = new uint[2];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(data[0], Is.EqualTo(1227133513));
        Assert.That(data[1], Is.EqualTo(1074341010));
    }

    [Test]
    public void Run_MoveD1Data()
    {
        // load D1, const
        // move #data, D1
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadD1Const,
            1227133513,
            1074341010,
            (uint)Mnem.MoveDataD1,
            0,
            (uint)Mnem.Halt,
        };
        var data = new uint[2];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(data[0], Is.EqualTo(1227133513));
        Assert.That(data[1], Is.EqualTo(1074341010));
    }

    [Test]
    public void Run_MoveDataToD0()
    {
        // move D0, #data
        // halt

        uint[] code = { (uint)Mnem.MoveD0Data, 0, (uint)Mnem.Halt };
        uint[] data = { 1227133513, 1074341010 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.D0, Is.EqualTo(3.1428571428571428));
    }

    [Test]
    public void Run_DecA()
    {
        // load A, const
        // dec A
        // halt

        uint[] code = { (uint)Mnem.LoadAConst, 1, (uint)Mnem.DecA, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(0));
        Assert.That(cpu.Flags & cpu.SetFlagMask.Z, Is.EqualTo(cpu.SetFlagMask.Z));
        Assert.That(cpu.Flags & cpu.SetFlagMask.S, Is.EqualTo(0x00));
        Assert.That(cpu.Flags & cpu.SetFlagMask.N, Is.EqualTo(cpu.SetFlagMask.N));
        Assert.That(cpu.Flags & cpu.SetFlagMask.P, Is.EqualTo(0x00));
    }

    [Test]
    public void Run_IncA()
    {
        // load A, const
        // inc A
        // halt

        uint[] code = { (uint)Mnem.LoadAConst, uint.MaxValue, (uint)Mnem.IncA, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(0));
        Assert.That(cpu.Flags & cpu.SetFlagMask.Z, Is.EqualTo(cpu.SetFlagMask.Z));
        Assert.That(cpu.Flags & cpu.SetFlagMask.S, Is.EqualTo(0x00));
        Assert.That(cpu.Flags & cpu.SetFlagMask.N, Is.EqualTo(0x00));
        Assert.That(cpu.Flags & cpu.SetFlagMask.P, Is.EqualTo(cpu.SetFlagMask.P));
    }

    [Test]
    public void Run_AddAConst()
    {
        // load A, const
        // add A, const
        // halt

        uint[] code = { (uint)Mnem.LoadAConst, 2, (uint)Mnem.AddAConst, 5, (uint)Mnem.Halt };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(7));
        Assert.That(cpu.Flags & cpu.SetFlagMask.Z, Is.EqualTo(0x00));
        Assert.That(cpu.Flags & cpu.SetFlagMask.S, Is.EqualTo(0x00));
        Assert.That(cpu.Flags & cpu.SetFlagMask.N, Is.EqualTo(0x00));
    }

    [Test]
    public void Run_AddAConst_Overflow()
    {
        // load A, const
        // add A, const
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadAConst,
            uint.MaxValue - 2,
            (uint)Mnem.AddAConst,
            5,
            (uint)Mnem.Halt,
        };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(2));
        Assert.That(cpu.Flags & cpu.SetFlagMask.P, Is.EqualTo(cpu.SetFlagMask.P));
    }

    [Test]
    public void Run_PushA()
    {
        // load SP, const
        // load A, const
        // push A
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            2,
            (uint)Mnem.LoadAConst,
            7,
            (uint)Mnem.PushA,
            (uint)Mnem.Halt,
        };
        var data = new uint[3];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.SP, Is.EqualTo(1));
        Assert.That(data[2], Is.EqualTo(7));
    }

    [Test]
    public void Run_PopA()
    {
        // load SP const
        // load A const
        // pop A
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            1,
            (uint)Mnem.LoadAConst,
            3,
            (uint)Mnem.PopA,
            (uint)Mnem.Halt,
        };
        uint[] data = { 0, 0, 7 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.SP, Is.EqualTo(2));
        Assert.That(cpu.A, Is.EqualTo(7));
    }

    [Test]
    public void Run_JumpNotZero()
    {
        // 0: load A, const
        // 2: dec A
        // 3: jpnz addr
        // 5: halt

        uint[] code =
        {
            (uint)Mnem.LoadAConst,
            5,
            (uint)Mnem.DecA,
            (uint)Mnem.JumpNotZero,
            2,
            (uint)Mnem.Halt,
        };
        uint[] data = { 0 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.A, Is.EqualTo(0));
    }

    [Test]
    public void Run_Call()
    {
        // 0: load SP const
        // 2: call addr
        // 4: nop
        // 5: halt

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            2,
            (uint)Mnem.Call,
            5,
            (uint)Mnem.Nop,
            (uint)Mnem.Halt,
        };
        var data = new uint[3];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(data[2], Is.EqualTo(4));
        Assert.That(cpu.SP, Is.EqualTo(1));
    }

    [Test]
    public void Run_Return()
    {
        // 0: load SP const
        // 2: ret
        // 3: nop
        // 4: halt

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            1,
            (uint)Mnem.Return,
            (uint)Mnem.Nop,
            (uint)Mnem.Halt,
        };
        uint[] data = { 0, 0, 4 };
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(cpu.SP, Is.EqualTo(2));
    }

    [Test]
    public void Run_CallReturn()
    {
        // -- int AddFive(int x) { return x + 5; }
        //
        // 00: load SP, const
        // 02: load A, const
        // 04: push A
        // 05: call addr
        // 07: halt
        // 08: pop B
        // 09: pop A
        // 10: add A, const
        // 12: push A
        // 13: push B
        // 14: ret

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            4,
            (uint)Mnem.LoadAConst,
            12,
            (uint)Mnem.PushA,
            (uint)Mnem.Call,
            8,
            (uint)Mnem.Halt,
            (uint)Mnem.PopB,
            (uint)Mnem.PopA,
            (uint)Mnem.AddAConst,
            5,
            (uint)Mnem.PushA,
            (uint)Mnem.PushB,
            (uint)Mnem.Return,
        };
        var data = new uint[5];
        var cpu = new Cpu(code, data);
        cpu.Run();
        Assert.That(data[4], Is.EqualTo(17));
    }

    [Test]
    public void Run_SysCall_ConsoleWriteline()
    {
        // load A, const
        // sys func
        // halt

        uint[] code = { (uint)Mnem.LoadAConst, 0, (uint)Mnem.Syscall, 0, (uint)Mnem.Halt };
        var data = new uint[5];
        const string message = "Hello World!";
        var msgBytes = Encoding.Default.GetBytes(message);

        var packer = new PrimitivePacker();
        var j = 0;
        for (var i = 0; i < msgBytes.Length; i += sizeof(uint)) // TODO alignment
        {
            packer.b0 = msgBytes[i];
            packer.b1 = msgBytes[i + 1];
            packer.b2 = msgBytes[i + 2];
            packer.b3 = msgBytes[i + 3];
            data[j] = packer.ui0;
            ++j;
        }
        data[j] = 0;

        var cpu = new Cpu(code, data);
        cpu.Run();
    }

    [Test]
    public void Run_IndirectAddressing()
    {
        // load SP, const
        // load A, const
        // load D, const
        // move #data, A[SP+D]
        // load A, const
        // move A, #data[SP+D]
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadSpConst,
            1,
            (uint)Mnem.LoadAConst,
            5,
            (uint)Mnem.LoadDConst,
            1,
            (uint)Mnem.MoveDataASPD,
            (uint)Mnem.LoadAConst,
            2,
            (uint)Mnem.MoveADataSPD,
            (uint)Mnem.Halt,
        };
        var data = new uint[3];
        var cpu = new Cpu(code, data);
        cpu.Run();

        Assert.That(data[2], Is.EqualTo(5));
        Assert.That(cpu.A, Is.EqualTo(5));
    }

    [Test]
    public void Run_AddB()
    {
        // load A, const
        // load B, const
        // add B
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadAConst,
            2,
            (uint)Mnem.LoadBConst,
            3,
            (uint)Mnem.AddB,
            (uint)Mnem.Halt,
        };
        uint[] data = { };
        var cpu = new Cpu(code, data);
        cpu.Run();

        Assert.That(cpu.A, Is.EqualTo(5));
    }

    [Test]
    public void Run_AndB()
    {
        // load A, const
        // load B, const
        // and B
        // halt

        uint[] code =
        {
            (uint)Mnem.LoadAConst,
            0x0f,
            (uint)Mnem.LoadBConst,
            0x04,
            (uint)Mnem.AndB,
            (uint)Mnem.Halt,
        };
        uint[] data = { };
        var cpu = new Cpu(code, data);
        cpu.Run();

        Assert.That(cpu.A, Is.EqualTo(0x04));
    }
}
