namespace Ryu64.MIPS
{
    public class COP1
    {
        public static bool COP1_ON = false;

        public static void PowerOnCOP1()
        {
            for (int i = 0; i < Registers.COP1.Reg.Length; ++i)
                Registers.COP1.Reg[i] = 0.0; // Clear Registers.

            COP1_ON = true;
        }
    }
}
