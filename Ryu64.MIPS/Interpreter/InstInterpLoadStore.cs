namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void LB(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (byte)R4300.memory.ReadInt8((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = R4300.memory.ReadUInt8((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LD(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)R4300.memory.ReadInt64((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LDL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] &= 0x00000000FFFFFFFF;
            Registers.R4300.Reg[Desc.op2] |= (ulong)R4300.memory.ReadInt64((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) << 32;
            Registers.R4300.PC += 4;
        }

        public static void LDR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] >>= 32;
            Registers.R4300.Reg[Desc.op2] |= (ulong)R4300.memory.ReadInt64((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) >> 32;
            Registers.R4300.PC += 4;
        }

        public static void LH(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ushort)R4300.memory.ReadInt16((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LHU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = R4300.memory.ReadUInt16((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LW(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)R4300.memory.ReadInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LWL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] &= 0xFFFFFFFF0000FFFF;
            Registers.R4300.Reg[Desc.op2] |= (uint)R4300.memory.ReadInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) << 16;
            Registers.R4300.PC += 4;
        }

        public static void LWR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] &= 0xFFFFFFFFFFFF0000;
            Registers.R4300.Reg[Desc.op2] |= (uint)R4300.memory.ReadInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) >> 16;
            Registers.R4300.PC += 4;
        }

        public static void LWU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = R4300.memory.ReadUInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void SB(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt8((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (byte)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SD(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt64((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SDL(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (uint)((Registers.R4300.Reg[Desc.op2]) >> 32));
            Registers.R4300.PC += 4;
        }

        public static void SDR(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 4, (uint)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SH(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt16((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (ushort)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SW(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt32((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (uint)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SWL(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteInt16((uint)((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (short)(Registers.R4300.Reg[Desc.op2] >> 16));
            Registers.R4300.PC += 4;
        }

        public static void SWR(OpcodeTable.OpcodeDesc Desc)
        {
            R4300.memory.WriteUInt16((uint)(((int)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 2), (ushort)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }
    }
}
