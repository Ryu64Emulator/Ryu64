﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class COP0
    {
        public static bool COP0_ON = false;

        public static void PowerOnCOP0()
        {
            for (int i = 0; i < Registers.COP0.Reg.Length; ++i)
                Registers.COP0.Reg[i] = 0; // Clear Registers

            Registers.COP0.Reg[Registers.COP0.COMPARE_REG] = 0xFFFFFFFF;

            if (!Common.Settings.LOAD_PIF)
            {
                Registers.COP0.Reg[Registers.COP0.STATUS_REG] = 0x34000000;
                Registers.COP0.Reg[Registers.COP0.COUNT_REG]  = 0x00036464;
            }
            else
            {
                Registers.COP0.Reg[Registers.COP0.STATUS_REG] = 0x00400004;
            }

            COP0_ON = true;
        }
    }
}
