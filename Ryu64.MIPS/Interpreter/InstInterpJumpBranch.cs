using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void BEQ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
            if (Registers.R4300.Reg[Desc.op1] == Registers.R4300.Reg[Desc.op2])
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
        }

        public static void BEQL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if (Registers.R4300.Reg[Desc.op1] == Registers.R4300.Reg[Desc.op2])
            {
                R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
            }
            else Registers.R4300.PC += 4;
        }

        public static void BGEZAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.Reg[31] = (long)Registers.R4300.PC;
            if (Registers.R4300.Reg[Desc.op1] >= 0)
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
        }

        public static void BLEZL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if (Registers.R4300.Reg[Desc.op1] <= 0)
            {
                R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
            }
            else Registers.R4300.PC += 4;
        }

        public static void BNE(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
            if (Registers.R4300.Reg[Desc.op1] != Registers.R4300.Reg[Desc.op2])
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
        }

        public static void BNEL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            if (Registers.R4300.Reg[Desc.op1] != Registers.R4300.Reg[Desc.op2])
            {
                R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
                Registers.R4300.PC += (ulong)((short)(Desc.Imm - 1) * 4);
            }
            else Registers.R4300.PC += 4;
        }

        public static void JAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.Reg[31] = (long)Registers.R4300.PC;
            Registers.R4300.PC = Desc.Target;
        }

        public static void JR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4;
            R4300.InterpretOpcode(Memory.ReadUInt32(Registers.R4300.PC));
            Registers.R4300.PC = (ulong)(Registers.R4300.Reg[Desc.op1] & 0xFFFFFFFF);
        }
    }
}
