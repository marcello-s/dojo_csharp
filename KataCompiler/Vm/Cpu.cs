#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataCompiler.Vm;

class Cpu
{
    // programm counter, stack pointer
    public uint PC { get; private set; }
    public uint SP { get; private set; }

    // integer registers, floating point registers, flags
    public uint A { get; private set; }
    public uint B { get; private set; }
    public uint C { get; private set; }
    public uint D { get; private set; }
    public uint E { get; private set; }
    public double D0 { get; private set; }
    public double D1 { get; private set; }
    public double D2 { get; private set; }
    public double D3 { get; private set; }
    public uint Flags { get; private set; }
    public FlagMask SetFlagMask { get; private set; }
    public FlagMask ResetFlagMask { get; private set; }

    // separate code (read) and data (read/write) memory
    // similar to Harvard processor architecure
    private readonly uint[] _code;
    private readonly uint[] _data;

    private PrimitivePacker _packer;
    private Bits _msbLsb;

    public Cpu(uint[] code, uint[] data)
    {
        _code = code;
        _data = data;
        _packer = new PrimitivePacker();

        SetFlagMask = new FlagMask(
            (uint)FlagSetMask.C,
            (uint)FlagSetMask.N,
            (uint)FlagSetMask.H,
            (uint)FlagSetMask.P,
            (uint)FlagSetMask.Z,
            (uint)FlagSetMask.S
        );

        _packer.i0 = (int)FlagResetMask.C;
        var resetFlagC = _packer.ui0;
        _packer.i0 = (int)FlagResetMask.N;
        var resetFlagN = _packer.ui0;
        _packer.i0 = (int)FlagResetMask.H;
        var resetFlagH = _packer.ui0;
        _packer.i0 = (int)FlagResetMask.P;
        var resetFlagP = _packer.ui0;
        _packer.i0 = (int)FlagResetMask.Z;
        var resetFlagZ = _packer.ui0;
        _packer.i0 = (int)FlagResetMask.S;
        var resetFlagS = _packer.ui0;

        ResetFlagMask = new FlagMask(
            resetFlagC,
            resetFlagN,
            resetFlagH,
            resetFlagP,
            resetFlagZ,
            resetFlagS
        );

        _msbLsb = new Bits((uint)MsbLsb.Lsb, (uint)MsbLsb.Msb);
    }

    public void Run()
    {
        while (true)
        {
            var halt = ProcessCycle(Fetch());

            if (halt)
            {
                break;
            }
        }
    }

    private uint Fetch()
    {
        return _code[PC++];
    }

    private bool ProcessCycle(uint inst)
    {
        var halt = false;
        uint a = 0;

        switch ((Mnem)inst)
        {
            // nop
            case Mnem.Nop:
                break;

            // load A, const
            case Mnem.LoadAConst:
                A = Fetch();
                break;

            // load B, const
            case Mnem.LoadBConst:
                B = Fetch();
                break;

            // load C, const
            case Mnem.LoadCConst:
                C = Fetch();
                break;

            // load D, const
            case Mnem.LoadDConst:
                D = Fetch();
                break;

            // load E, const
            case Mnem.LoadEConst:
                E = Fetch();
                break;

            // load D0, const
            case Mnem.LoadD0Const:
                _packer.ui0 = Fetch();
                _packer.ui1 = Fetch();
                D0 = _packer.d0;
                break;

            // load D1, const
            case Mnem.LoadD1Const:
                _packer.ui0 = Fetch();
                _packer.ui1 = Fetch();
                D1 = _packer.d0;
                break;

            // load D2, const
            case Mnem.LoadD2Const:
                _packer.ui0 = Fetch();
                _packer.ui1 = Fetch();
                D2 = _packer.d0;
                break;

            // load D3, const
            case Mnem.LoadD3Const:
                _packer.ui0 = Fetch();
                _packer.ui1 = Fetch();
                D3 = _packer.d0;
                break;

            // load SP, const
            case Mnem.LoadSpConst:
                SP = Fetch();
                break;

            // move #data, D0
            case Mnem.MoveDataD0:
                MoveToData(D0, Fetch());
                break;

            // move #data, D1
            case Mnem.MoveDataD1:
                MoveToData(D1, Fetch());
                break;

            // move #data, D2
            case Mnem.MoveDataD2:
                MoveToData(D2, Fetch());
                break;

            // move #data, D3
            case Mnem.MoveDataD3:
                MoveToData(D3, Fetch());
                break;

            // move #data, A[SP+D]
            case Mnem.MoveDataASPD:
                _data[SP + D] = A;
                break;

            // move D0, #data
            case Mnem.MoveD0Data:
                D0 = MoveDataTo(Fetch());
                break;

            // move D1, #data
            case Mnem.MoveD1Data:
                D1 = MoveDataTo(Fetch());
                break;

            // move D2, #data
            case Mnem.MoveD2Data:
                D2 = MoveDataTo(Fetch());
                break;

            // move D3, #data
            case Mnem.MoveD3Data:
                D3 = MoveDataTo(Fetch());
                break;

            // move A, #data[SP+D]
            case Mnem.MoveADataSPD:
                A = _data[SP + D];
                break;

            // dec A
            case Mnem.DecA:
                a = A;
                --A;
                Flags |= SetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A > a);
                break;

            // inc A
            case Mnem.IncA:
                a = A;
                ++A;
                Flags &= ResetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // add A, const
            case Mnem.AddAConst:
                a = A;
                A += Fetch();
                Flags &= ResetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // add B
            case Mnem.AddB:
                a = A;
                A += B;
                Flags &= ResetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // sub B
            case Mnem.SubB:
                a = A;
                A -= B;
                Flags |= SetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A > a);
                break;

            // neg A
            case Mnem.NegA:
                a = A;
                A = ~A;
                Flags &= ResetFlagMask.N;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // and B
            case Mnem.AndB:
                a = A;
                A &= B;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // or B
            case Mnem.OrB:
                a = A;
                A |= B;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // xor B
            case Mnem.XorB:
                a = A;
                A ^= B;
                SetFlagsForResult(A);
                SetFlagsForArithmeticResult(A < a);
                break;

            // push A
            case Mnem.PushA:
                _data[SP] = A;
                --SP;
                break;

            // push B
            case Mnem.PushB:
                _data[SP] = B;
                --SP;
                break;

            // pop A
            case Mnem.PopA:
                ++SP;
                A = _data[SP];
                break;

            // pop B
            case Mnem.PopB:
                ++SP;
                B = _data[SP];
                break;

            // jpnz addr
            case Mnem.JumpNotZero:
                var addr = Fetch();

                if ((Flags & SetFlagMask.Z) != SetFlagMask.Z)
                {
                    PC = addr;
                }
                break;

            // call addr
            case Mnem.Call:
                var callAddr = Fetch();
                _data[SP] = PC;
                --SP;
                PC = callAddr;
                break;

            // ret
            case Mnem.Return:
                ++SP;
                PC = _data[SP];
                break;

            case Mnem.Syscall:
                var sysCallFunction = Fetch();
                DispatchSysCall(sysCallFunction);
                break;

            // halt
            case Mnem.Halt:
            default:
                halt = true;
                break;
        }

        return halt;
    }

    private void MoveToData(double d, uint dp)
    {
        _packer.d0 = d;
        _data[dp++] = _packer.ui0;
        _data[dp] = _packer.ui1;
    }

    private double MoveDataTo(uint dp)
    {
        _packer.ui0 = _data[dp++];
        _packer.ui1 = _data[dp];
        return _packer.d0;
    }

    private void SetFlagsForResult(uint result)
    {
        if (result == 0)
        {
            Flags |= SetFlagMask.Z;
        }

        if ((result & _msbLsb.Msb) == _msbLsb.Msb)
        {
            Flags |= SetFlagMask.S;
        }
        else
        {
            Flags &= ResetFlagMask.S;
        }
    }

    private void SetFlagsForArithmeticResult(bool overflow)
    {
        if (overflow)
        {
            Flags |= SetFlagMask.P;
        }
        else
        {
            Flags &= ResetFlagMask.P;
        }
    }

    [Flags]
    private enum FlagSetMask : uint
    {
        // S, Z, P/V, H, N, C

        // S = sign flag, 1 if MSB of result is 1
        // Z = zero flag, 1 if result is 0
        // P/N = parity/overflow, parity set by logical operations, overflow set by arithmetics
        // H = half carry flag, 1 if add/subtract produced carry into or borrow from
        // N = add/subtract flag, 1 if previous operation was subtract
        // C = carry flag, 1 if oepration produced a carry from MSB of the result

        C = 0x01,
        N = 0x02,
        H = 0x04,
        P = 0x08,
        Z = 0x10,
        S = 0x20,
    }

    [Flags]
    private enum FlagResetMask
    {
        C = ~0x01,
        N = ~0x02,
        H = ~0x04,
        P = ~0x08,
        Z = ~0x10,
        S = ~0x20,
    }

    public struct FlagMask
    {
        // S, Z, P/V, H, N, C

        // S = sign flag, 1 if MSB of result is 1
        // Z = zero flag, 1 if result is 0
        // P/N = parity/overflow, parity set by logical operations, overflow set by arithmetics
        // H = half carry flag, 1 if add/subtract produced carry into or borrow from
        // N = add/subtract flag, 1 if previous operation was subtract
        // C = carry flag, 1 if operation produced a carry from MSB of the result

        public uint C { get; private set; }
        public uint N { get; private set; }
        public uint H { get; private set; }
        public uint P { get; private set; }
        public uint Z { get; private set; }
        public uint S { get; private set; }

        public FlagMask(uint c, uint n, uint h, uint p, uint z, uint s)
            : this()
        {
            C = c;
            N = n;
            H = h;
            P = p;
            Z = z;
            S = s;
        }
    }

    [Flags]
    private enum MsbLsb : uint
    {
        Lsb = 0x01,
        Msb = 0x10000000,
    }

    private struct Bits
    {
        public uint Lsb { get; private set; }
        public uint Msb { get; private set; }

        public Bits(uint lsb, uint msb)
            : this()
        {
            Lsb = lsb;
            Msb = msb;
        }
    }

    private void DispatchSysCall(uint sysCallFunction)
    {
        switch (sysCallFunction)
        {
            // console writeline string terminated by zero (sz)
            // A = data start
            case 0:
                var start = A;
                var end = Array.FindIndex(_data, (int)start, u => u == 0);
                var bytes = new byte[(end - start) * sizeof(uint)]; // TODO alignment
                var j = 0;
                for (var i = 0; i < (end - start); ++i)
                {
                    _packer.ui0 = _data[i];
                    bytes[j] = _packer.b0;
                    bytes[j + 1] = _packer.b1;
                    bytes[j + 2] = _packer.b2;
                    bytes[j + 3] = _packer.b3;
                    j += sizeof(uint);
                }
                var theString = Encoding.Default.GetString(bytes);
                Console.WriteLine(theString);
                break;

            default:
                break;
        }
    }
}
