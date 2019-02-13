using System;

namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void ADD(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            ADDU(Desc);
        }

        public static void ADDI(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            ADDIU(Desc);
        }

        public static void ADDIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void ADDU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU)), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void DADD(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            DADDU(Desc);
        }

        public static void DADDI(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for Overflow and Underflow
            DADDIU(Desc);
        }

        public static void DADDIU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (ulong)((long)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void DADDU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void AND(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) & (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void ANDI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) & Desc.Imm, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SUB(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for underflow
            SUBU(Desc);
        }

        public static void DSUB(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for underflow\
            DSUBU(Desc);
        }

        public static void SUBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) - (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void DSUBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op1] - Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void NOR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)~((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) | (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU)), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void XOR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) ^ (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void XORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) ^ Desc.Imm, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LUI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (uint)(Desc.Imm << 16) & 0xFFFF0000, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void MFHI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.HI;
            Registers.R4300.PC += 4;
        }

        public static void MFLO(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.LO;
            Registers.R4300.PC += 4;
        }

        public static void OR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) | (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void ORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) | Desc.Imm, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void MTLO(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.LO = Registers.R4300.Reg[Desc.op1];
            Registers.R4300.PC += 4;
        }

        public static void MTHI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.HI = Registers.R4300.Reg[Desc.op1];
            Registers.R4300.PC += 4;
        }

        public static void MULT(OpcodeTable.OpcodeDesc Desc)
        {
            ulong Res = (ulong)((int)Registers.R4300.Reg[Desc.op1] * (int)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.LO = (uint)(Res & 0xFFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void MULTU(OpcodeTable.OpcodeDesc Desc)
        {
            ulong Res = (uint)Registers.R4300.Reg[Desc.op1] * (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.LO = (uint)(Res & 0xFFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void DMULTU(OpcodeTable.OpcodeDesc Desc)
        {
            ulong Res = Registers.R4300.Reg[Desc.op1] * Registers.R4300.Reg[Desc.op2];
            Registers.R4300.LO = (uint)(Res & 0xFFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void DIV(OpcodeTable.OpcodeDesc Desc)
        {
            DIVU(Desc);
        }

        public static void DIVU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.LO = (uint)Registers.R4300.Reg[Desc.op1] / (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.HI = (uint)Registers.R4300.Reg[Desc.op1] % (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void DDIV(OpcodeTable.OpcodeDesc Desc)
        {
            DDIVU(Desc);
        }

        public static void DDIVU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.LO = Registers.R4300.Reg[Desc.op1] / Registers.R4300.Reg[Desc.op2];
            Registers.R4300.HI = Registers.R4300.Reg[Desc.op1] % Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void SLL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)(Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU) << Desc.op4), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void DSLL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op2] << Desc.op4;
            Registers.R4300.PC += 4;
        }

        public static void DSLL32(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op2] << (Desc.op4 + 32);
            Registers.R4300.PC += 4;
        }

        public static void SLLV(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU) << (byte)(Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) & 0x1F), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        // Currently Shift Arithmetic instructions are not implemented correctly.
        public static void SRA(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU) >> Desc.op4, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void DSRA32(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op2] >> (Desc.op4 + 32);
            Registers.R4300.PC += 4;
        }

        public static void DSRL32(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op2] >> (Desc.op4 + 32);
            Registers.R4300.PC += 4;
        }

        public static void SRL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU) >> Desc.op4, Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SRLV(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op3, (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU) >> (byte)(Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) & 0x1F), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SLTI(OpcodeTable.OpcodeDesc Desc)
        {
            if ((long)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) < (short)Desc.Imm)
            {
                Registers.SetMainReg(Desc.op2, 1, Desc.RSP, Desc.CPU);
            }
            else
            {
                Registers.SetMainReg(Desc.op2, 0, Desc.RSP, Desc.CPU);
            }

            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SLTIU(OpcodeTable.OpcodeDesc Desc)
        {
            if (Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) < Desc.Imm)
            {
                Registers.SetMainReg(Desc.op2, 1, Desc.RSP, Desc.CPU);
            }
            else
            {
                Registers.SetMainReg(Desc.op2, 0, Desc.RSP, Desc.CPU);
            }

            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SLT(OpcodeTable.OpcodeDesc Desc)
        {
            if ((long)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) < (long)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU))
            {
                Registers.SetMainReg(Desc.op3, 1, Desc.RSP, Desc.CPU);
            }
            else
            {
                Registers.SetMainReg(Desc.op3, 0, Desc.RSP, Desc.CPU);
            }

            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SLTU(OpcodeTable.OpcodeDesc Desc)
        {
            if (Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) < Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU))
            {
                Registers.SetMainReg(Desc.op3, 1, Desc.RSP, Desc.CPU);
            }
            else
            {
                Registers.SetMainReg(Desc.op3, 0, Desc.RSP, Desc.CPU);
            }

            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }
    }
}
