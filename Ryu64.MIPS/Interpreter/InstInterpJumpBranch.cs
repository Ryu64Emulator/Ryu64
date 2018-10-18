namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void BEQ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] == (long)Registers.R4300.Reg[Desc.op2])
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BEQL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if ((long)Registers.R4300.Reg[Desc.op1] == (long)Registers.R4300.Reg[Desc.op2])
            {
                R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
            }
            else Registers.R4300.PC += 4;
        }

        public static void BGEZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] >= 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BGEZAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.Reg[31] = Registers.R4300.PC;
            if ((long)Registers.R4300.Reg[Desc.op1] >= 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BGTZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] > 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BLEZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] <= 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BLEZL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if ((long)Registers.R4300.Reg[Desc.op1] <= 0)
            {
                R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
            }
            else Registers.R4300.PC += 4;
        }

        public static void BLTZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] <= 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BLTZAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.Reg[31] = Registers.R4300.PC;
            if ((long)Registers.R4300.Reg[Desc.op1] <= 0)
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BNE(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            if ((long)Registers.R4300.Reg[Desc.op1] != (long)Registers.R4300.Reg[Desc.op2])
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
        }

        public static void BNEL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if ((long)Registers.R4300.Reg[Desc.op1] != (long)Registers.R4300.Reg[Desc.op2])
            {
                R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (uint)((short)((Desc.Imm - 1) << 2));
            }
            else Registers.R4300.PC += 4;
        }

        public static void J(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.PC = ((Registers.R4300.PC - 4) & 0xF0000000) | (Desc.Target << 2);
        }

        public static void JAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.Reg[31] = Registers.R4300.PC;
            Registers.R4300.PC = ((Registers.R4300.PC - 4) & 0xF0000000) | (Desc.Target << 2);
        }

        public static void JR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.PC = (uint)(Registers.R4300.Reg[Desc.op1] & 0xFFFFFFFF);
        }
    }
}
