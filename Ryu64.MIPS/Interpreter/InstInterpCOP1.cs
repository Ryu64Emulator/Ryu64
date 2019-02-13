using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void SDC1(OpcodeTable.OpcodeDesc Desc)
        {
            uint Address = (uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm);
            if (Registers.COP0.FR_STATUS)
            {
                Cores.R4300.memory.WriteUInt64(Address, Registers.COP1.Reg[Desc.op2]);
            }
            else
            {
                Cores.R4300.memory.WriteUInt32(Address,     (uint)Registers.COP1.Reg[Desc.op2]);
                Cores.R4300.memory.WriteUInt32(Address + 1, (uint)Registers.COP1.Reg[Desc.op2 + 1]);
            }

            Registers.R4300.PC += 4;
        }

        public static void LDC1(OpcodeTable.OpcodeDesc Desc)
        {
            uint Address = (uint)((uint)Registers.R4300.Reg[Desc.op1] + (short)Desc.Imm);
            if (Registers.COP0.FR_STATUS)
            {
                Registers.COP1.Reg[Desc.op2] = Cores.R4300.memory.ReadUInt64(Address);
            }
            else
            {
                Registers.COP1.Reg[Desc.op2]     = Cores.R4300.memory.ReadUInt32(Address);
                Registers.COP1.Reg[Desc.op2 + 1] = Cores.R4300.memory.ReadUInt32(Address + 1);
            }

            Registers.R4300.PC += 4;
        }

        public static void DMTC1(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op3 % 2 != 0 && !Registers.COP0.FR_STATUS) throw new Common.Exceptions.InvalidOperationException("DMTC1: When the FR bit is disabled only even numbered Floating-Point registers are usable.");

            Registers.COP1.Reg[Desc.op3] = Registers.R4300.Reg[Desc.op2];

            Registers.R4300.PC += 4;
        }

        public static void CFC1(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op3 == 0 || Desc.op3 == 31)
            {
                Registers.R4300.Reg[Desc.op2] = (uint)Registers.COP1.Reg[Desc.op3];
            }
            else
            {
                throw new InvalidOperationException($"CFC1: fs cannot be {Desc.op3}!");
            }

            Registers.R4300.PC += 4;
        }

        public static void CTC1(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op3 == 0 || Desc.op3 == 31)
            {
                Registers.COP1.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2];
            }
            else
            {
                throw new InvalidOperationException($"CTC1: fs cannot be {Desc.op3}!");
            }

            Registers.R4300.PC += 4;
        }
    }
}
