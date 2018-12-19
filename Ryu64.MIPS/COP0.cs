namespace Ryu64.MIPS
{
    public class COP0
    {
        public static bool COP0_ON = false;

        public static void PowerOnCOP0()
        {
            for (int i = 0; i < Registers.COP0.Reg.Length; ++i)
                Registers.COP0.Reg[i] = 0; // Clear Registers.

            Registers.COP0.Reg[Registers.COP0.COMPARE_REG] = 0xFFFFFFFF;
            Registers.COP0.Reg[Registers.COP0.STATUS_REG]  = 0x34000000;
            Registers.COP0.Reg[Registers.COP0.CONFIG_REG]  = 0x0006E463;

            COP0_ON = true;
        }
    }
}
