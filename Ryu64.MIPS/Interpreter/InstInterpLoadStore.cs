namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void LB(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (byte)R4300.memory.ReadInt8((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, R4300.memory.ReadUInt8((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LD(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)R4300.memory.ReadInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LLD(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)R4300.memory.ReadInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.LLbit = 1;
            Registers.R4300.PC += 4;
        }

        public static void LDL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] >>= 32;
            Registers.R4300.Reg[Desc.op2] |= (ulong)R4300.memory.ReadInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) << 32;
            Registers.R4300.PC += 4;
        }

        public static void LDR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] &= ~0xFFFFFFFF;
            Registers.R4300.Reg[Desc.op2] |= (ulong)R4300.memory.ReadInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) >> 32;
            Registers.R4300.PC += 4;
        }

        public static void LH(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, (ushort)R4300.memory.ReadInt16((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LHU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, R4300.memory.ReadUInt16((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LW(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.SetMainReg(Desc.op2, R4300.memory.ReadUInt32((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void LWL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] |= R4300.memory.ReadUInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) << 16;
            Registers.R4300.PC += 4;
        }

        public static void LWR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] |= R4300.memory.ReadUInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) >> 16;
            Registers.R4300.PC += 4;
        }

        public static void LWU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = R4300.memory.ReadUInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void SB(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt8((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), (byte)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SD(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SDL(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (uint)((Registers.R4300.Reg[Desc.op2]) >> 32));
            Registers.R4300.PC += 4;
        }

        public static void SDR(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 4, (uint)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SCD(OpcodeTable.OpcodeDesc Desc)
        {
            if (Registers.R4300.LLbit == 1)
                R4300.memory.WriteUInt64((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.Reg[Desc.op2] = Registers.R4300.LLbit;
            Registers.R4300.PC += 4;
        }

        public static void SH(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt16((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), (ushort)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SW(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) + (short)Desc.Imm), (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void SWL(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt16((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (ushort)(Registers.R4300.Reg[Desc.op2] >> 16));
            Registers.R4300.PC += 4;
        }

        public static void SWR(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt16((uint)(((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 2), (ushort)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }
    }
}
