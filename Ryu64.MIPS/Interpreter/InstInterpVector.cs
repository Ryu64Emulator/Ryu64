using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void LQV(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.RSPCOP2.Reg[Desc.op2].LoadQuadword(Cores.R4300.memory[(uint)(((uint)Registers.RSPReg.Reg[Desc.op1] + (sbyte)Desc.VOff) & 0xFFF) + 0x04000000, 16]);
            Registers.RSPReg.PC += 4;
        }

        public static void SQV(OpcodeTable.OpcodeDesc Desc)
        {
            Cores.R4300.memory[(uint)(((uint)Registers.RSPReg.Reg[Desc.op1] + (sbyte)Desc.VOff) & 0xFFF) + 0x04000000, 16] = Registers.RSPCOP2.Reg[Desc.op2].Vector;
            Registers.RSPReg.PC += 4;
        }

        public static void VSUBC(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.RSPCOP2.Reg[Desc.op4].SubWithCarry(Registers.RSPCOP2.Reg[Desc.op3], Registers.RSPCOP2.Reg[Desc.op2], Desc.Ve);
            Registers.RSPReg.PC += 4;
        }
    }
}
