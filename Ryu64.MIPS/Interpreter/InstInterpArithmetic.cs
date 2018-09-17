using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void ADD(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            int Res = (int)((Registers.R4300.Reg[Desc.op1] & 0xFFFFFFFF) + (Registers.R4300.Reg[Desc.op2] & 0xFFFFFFFF));
            Registers.R4300.Reg[Desc.op3] = Res;
            Registers.R4300.PC += 4;
        }

        public static void ADDI(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            int Res = (int)((Registers.R4300.Reg[Desc.op1] & 0xFFFFFFFF) + (short)Desc.Imm);
            Registers.R4300.Reg[Desc.op2] = Res;
            Registers.R4300.PC += 4;
        }

        public static void ADDIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (int)((int)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) & 0xFFFFFFFF);
            Registers.R4300.PC += 4;
        }

        public static void XOR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op1] ^ Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void LUI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (short)Desc.Imm;
            Registers.R4300.PC += 4;
        }

        public static void OR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op1] | Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void ORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Registers.R4300.Reg[Desc.op1] | Desc.Imm;
            Registers.R4300.PC += 4;
        }
    }
}
