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
            uint Res = (uint)((int)Registers.R4300.Reg[Desc.op1] + (int)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.Reg[Desc.op3] = Res;
            Registers.R4300.PC += 4;
        }

        public static void ADDI(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            uint Res = (uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm);
            Registers.R4300.Reg[Desc.op2] = Res;
            Registers.R4300.PC += 4;
        }

        public static void ADDIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm);
            Registers.R4300.PC += 4;
        }

        public static void AND(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op1] & Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void ANDI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Registers.R4300.Reg[Desc.op1] & Desc.Imm;
            Registers.R4300.PC += 4;
        }

        public static void XOR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op1] ^ Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void XORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Registers.R4300.Reg[Desc.op1] ^ Desc.Imm;
            Registers.R4300.PC += 4;
        }

        public static void LUI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)(Desc.Imm << 16);
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

        public static void SLL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (int)Registers.R4300.Reg[Desc.op2] << Desc.op4;
            Registers.R4300.PC += 4;
        }

        public static void SRL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (int)Registers.R4300.Reg[Desc.op2] >> Desc.op4;
            Registers.R4300.PC += 4;
        }

        public static void SLTI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Registers.R4300.Reg[Desc.op1] < (short)Desc.Imm ? 1 : 0;
            Registers.R4300.PC += 4;
        }

        public static void SLTIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Registers.R4300.Reg[Desc.op1] < Desc.Imm ? 1 : 0;
            Registers.R4300.PC += 4;
        }

        public static void SLTU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (ulong)Registers.R4300.Reg[Desc.op1] < (ulong)Registers.R4300.Reg[Desc.op2] ? 1 : 0;
            Registers.R4300.PC += 4;
        }
    }
}
