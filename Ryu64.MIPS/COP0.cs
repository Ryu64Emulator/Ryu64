using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class COP0
    {
        public static void PowerOnCOP0()
        {
            for (int i = 0; i < Registers.COP0.Reg.Length; ++i)
                Registers.COP0.Reg[i] = 0; // Clear Registers
        }
    }
}
