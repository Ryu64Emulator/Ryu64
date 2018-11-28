namespace Ryu64.MIPS
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
            Registers.R4300.Reg[Desc.op2] = (uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm);
            Registers.R4300.PC += 4;
        }

        public static void ADDU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)((int)Registers.R4300.Reg[Desc.op1] + (int)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void AND(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op1] & (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void ANDI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)Registers.R4300.Reg[Desc.op1] & Desc.Imm;
            Registers.R4300.PC += 4;
        }

        public static void SUB(OpcodeTable.OpcodeDesc Desc)
        {
            // TODO: Correctly check for underflow
            SUBU(Desc);
        }

        public static void SUBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)((int)Registers.R4300.Reg[Desc.op1] - (int)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void XOR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op1] ^ (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void XORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)Registers.R4300.Reg[Desc.op1] ^ Desc.Imm;
            Registers.R4300.PC += 4;
        }

        public static void LUI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)(Desc.Imm << 16);
            Registers.R4300.PC += 4;
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
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op1] | (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void ORI(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)Registers.R4300.Reg[Desc.op1] | Desc.Imm;
            Registers.R4300.PC += 4;
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
            Registers.R4300.LO = (uint)(Res & 0x00000000FFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void MULTU(OpcodeTable.OpcodeDesc Desc)
        {
            ulong Res = (uint)Registers.R4300.Reg[Desc.op1] * (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.LO = (uint)(Res & 0x00000000FFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void DMULTU(OpcodeTable.OpcodeDesc Desc)
        {
            ulong Res = Registers.R4300.Reg[Desc.op1] * Registers.R4300.Reg[Desc.op2];
            Registers.R4300.LO = (uint)(Res & 0x00000000FFFFFFFF);
            Registers.R4300.HI = (uint)(Res >> 32);
            Registers.R4300.PC += 4;
        }

        public static void SLL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2] << Desc.op4;
            Registers.R4300.PC += 4;
        }

        public static void SLLV(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2] << (byte)(Registers.R4300.Reg[Desc.op1] & 0x0000001F);
            Registers.R4300.PC += 4;
        }

        public static void SRA(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (ulong)((int)Registers.R4300.Reg[Desc.op2] >> Desc.op4);
            Registers.R4300.PC += 4;
        }

        public static void SRL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2] >> Desc.op4;
            Registers.R4300.PC += 4;
        }

        public static void SRLV(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2] >> (byte)(Registers.R4300.Reg[Desc.op1] & 0x0000001F);
            Registers.R4300.PC += 4;
        }

        public static void SLTI(OpcodeTable.OpcodeDesc Desc)
        {
            if ((long)Registers.R4300.Reg[Desc.op1] < (short)Desc.Imm)
            {
                Registers.R4300.Reg[Desc.op2] = 1;
            }
            else
            {
                Registers.R4300.Reg[Desc.op2] = 0;
            }

            Registers.R4300.PC += 4;
        }

        public static void SLTIU(OpcodeTable.OpcodeDesc Desc)
        {
            if (Registers.R4300.Reg[Desc.op1] < Desc.Imm)
            {
                Registers.R4300.Reg[Desc.op2] = 1;
            }
            else
            {
                Registers.R4300.Reg[Desc.op2] = 0;
            }

            Registers.R4300.PC += 4;
        }

        public static void SLT(OpcodeTable.OpcodeDesc Desc)
        {
            if ((long)Registers.R4300.Reg[Desc.op1] < (long)Registers.R4300.Reg[Desc.op2])
            {
                Registers.R4300.Reg[Desc.op3] = 1;
            }
            else
            {
                Registers.R4300.Reg[Desc.op3] = 0;
            }

            Registers.R4300.PC += 4;
        }

        public static void SLTU(OpcodeTable.OpcodeDesc Desc)
        {
            if (Registers.R4300.Reg[Desc.op1] < Registers.R4300.Reg[Desc.op2])
            {
                Registers.R4300.Reg[Desc.op3] = 1;
            }
            else
            {
                Registers.R4300.Reg[Desc.op3] = 0;
            }

            Registers.R4300.PC += 4;
        }
    }
}
