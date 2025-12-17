#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler.Vm;

enum Mnem : uint
{
    Nop,
    LoadAConst,
    LoadBConst,
    LoadCConst,
    LoadDConst,
    LoadEConst,
    LoadD0Const,
    LoadD1Const,
    LoadD2Const,
    LoadD3Const,
    LoadSpConst,
    MoveDataD0,
    MoveDataD1,
    MoveDataD2,
    MoveDataD3,
    MoveDataASPD,
    MoveD0Data,
    MoveD1Data,
    MoveD2Data,
    MoveD3Data,
    MoveADataSPD,
    DecA,
    IncA,
    AddAConst,
    AddB,
    SubB,
    NegA,
    AndB,
    OrB,
    XorB,
    PushA,
    PushB,
    PopA,
    PopB,
    JumpNotZero,
    Call,
    Return,
    Syscall,
    Halt,
}
