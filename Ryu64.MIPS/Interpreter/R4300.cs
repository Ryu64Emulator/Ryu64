using System;

namespace Ryu64.MIPS
{
    public class R4300
    {
        public static void InterpretOpcode(uint Opcode)
        {
            OpcodeTable.GetInterpreterMethod(Opcode)(new OpcodeTable.OpcodeDesc(Opcode));
        }

        public static void PowerOnR4300()
        {
            for (int i = 0; i < Registers.R4300.Reg.Length; ++i)
                Registers.R4300.Reg[i] = 0; // Clear Registers

            COP0.PowerOnCOP0();
            COP1.PowerOnCOP1();

            OpcodeTable.Init();
        }
    }
}
