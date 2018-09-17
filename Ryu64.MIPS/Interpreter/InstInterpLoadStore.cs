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
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt8((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LBU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt8((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LD(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt64((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LDL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt64((ulong)(Desc.op1 + Desc.Imm)) & 0xFFFFFFFF00000000;
            Registers.R4300.PC += 4;
        }

        public static void LDR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt64((ulong)(Desc.op1 + Desc.Imm)) & 0x00000000FFFFFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LH(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt16((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LHU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt16((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LW(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt32((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void LWL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt16((ulong)(Desc.op1 + Desc.Imm)) & 0xFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LWR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (ulong)Memory.ReadInt16((ulong)((Desc.op1 + Desc.Imm) + 2)) & 0xFFFF;
            Registers.R4300.PC += 4;
        }

        public static void LWU(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = Memory.ReadUInt32((ulong)(Desc.op1 + Desc.Imm));
            Registers.R4300.PC += 4;
        }

        public static void SB(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt8((ulong)(Desc.op1 + Desc.Imm), (sbyte)(Registers.R4300.Reg[Desc.op2] & 0xFF));
            Registers.R4300.PC += 4;
        }

        public static void SD(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt64((ulong)(Desc.op1 + Desc.Imm), (long)Registers.R4300.Reg[Desc.op2]);
            Registers.R4300.PC += 4;
        }

        public static void SDL(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt32((ulong)(Desc.op1 + Desc.Imm), (int)((Registers.R4300.Reg[Desc.op2] & 0xFFFFFFFF00000000) >> 32));
            Registers.R4300.PC += 4;
        }

        public static void SDR(OpcodeTable.OpcodeDesc Desc)
        {
            Memory.WriteInt32((ulong)((Desc.op1 + Desc.Imm) + 2), (int)(Registers.R4300.Reg[Desc.op2] & 0x00000000FFFFFFFF));
            Registers.R4300.PC += 4;
        }
    }
}
