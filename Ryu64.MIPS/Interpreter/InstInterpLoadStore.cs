using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void LB(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt8((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt8((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LD(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt64((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LDL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (long)((ulong)Memory.ReadInt64((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) & 0xFFFFFFFF00000000);
            Registers.R4300.PC += 4;
        }

        public static void LDR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt64((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) & 0x00000000FFFFFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LH(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt16((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LHU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt16((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LW(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt32((uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LWL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt16((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm)) & 0xFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LWR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadInt16((ulong)((Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 2)) & 0xFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LWU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt32((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void SB(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt8((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (sbyte)(Registers.R4300.Reg[Desc.op2] & 0xFF));
            Registers.R4300.PC += 4;
        }

        public static void SD(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt64((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (long)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SDL(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt32((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (int)(((ulong)Registers.R4300.Reg[Desc.op2] & 0xFFFFFFFF00000000) >> 32));
            Registers.R4300.PC += 4;
        }

        public static void SDR(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt32((ulong)((Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 4), (int)(Registers.R4300.Reg[Desc.op2] & 0x00000000FFFFFFFF));
            Registers.R4300.PC += 4;
        }

        public static void SH(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt16((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (short)(Registers.R4300.Reg[Desc.op2] & 0xFFFF));
            Registers.R4300.PC += 4;
        }

        public static void SW(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt32((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (int)(Registers.R4300.Reg[Desc.op2] & 0xFFFFFFFF));
            Registers.R4300.PC += 4;
        }

        public static void SWL(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt16((ulong)(Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm), (short)((Registers.R4300.Reg[Desc.op2] & 0xFFFF0000) >> 16));
            Registers.R4300.PC += 4;
        }

        public static void SWR(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt16((ulong)((Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm) + 2), (short)(Registers.R4300.Reg[Desc.op2] & 0xFFFF));
            Registers.R4300.PC += 4;
        }
    }
}
