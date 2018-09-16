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
        }
    }
}
