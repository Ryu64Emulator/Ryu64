namespace Ryu64.MIPS
{
    public class MI
    {
        public const byte VI_Intr_Bit = 3;

        public static void PollVIInterrupt(uint CurrentScanline)
        {
            if ((R4300.memory.ReadUInt32(0x0430000C) & (1 << VI_Intr_Bit)) > 0)
            {
                if (CurrentScanline == R4300.memory.ReadUInt32(0x0440000C))
                    R4300.memory.InvokeMIInt(VI_Intr_Bit);
            }
        }
    }
}
