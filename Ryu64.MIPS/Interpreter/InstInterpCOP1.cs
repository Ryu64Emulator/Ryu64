using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void CFC1(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op3 == 0 || Desc.op3 == 31)
            {
                Registers.R4300.Reg[Desc.op2] = Common.Util.DoubleToUInt64(Registers.COP1.Reg[Desc.op3]) & 0xFFFFFFFF;
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
                Registers.COP1.Reg[Desc.op3] = Common.Util.UInt64ToDouble((uint)Registers.R4300.Reg[Desc.op2]);
            }
            else
            {
                throw new InvalidOperationException($"CTC1: fs cannot be {Desc.op3}!");
            }

            Registers.R4300.PC += 4;
        }
    }
}
