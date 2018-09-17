using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void ADDIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)((Registers.R4300.Reg[Desc.op1] + Desc.Imm) & 0xFFFFFFFF);
            Registers.R4300.PC += 4;
        }

        public static void LUI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Desc.Imm;
            Registers.R4300.PC += 4;
        }
    }
}
