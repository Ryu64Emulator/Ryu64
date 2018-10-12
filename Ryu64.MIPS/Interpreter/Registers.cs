namespace Ryu64.MIPS
{
    public class Registers
    {
        public class R4300
        {
            public static ulong[] Reg = new ulong[32];
            public static ulong  HI;
            public static ulong  LO;
            public static uint   PC;

            public static void PrintRegisterInfo()
            {
                for (int i = 0; i < Reg.Length; ++i)
                    Common.Logger.PrintInfoLine($"R[{i}]: 0x{Reg[i]:x16}");
            }
        }

        public class COP0
        {
            public const int INDEX_REG     = 0x0;
            public const int RANDOM_REG    = 0x1;
            public const int ENTRYLO0_REG  = 0x2;
            public const int ENTRYLO1_REG  = 0x3;
            public const int CONTEXT_REG   = 0x4;
            public const int PAGEMASK_REG  = 0x5;
            public const int WIRED_REG     = 0x6;
            public const int RESERVED0_REG = 0x7;
            public const int BADVADDR_REG  = 0x8;
            public const int COUNT_REG     = 0x9;
            public const int ENTRYHI_REG   = 0xA;
            public const int COMPARE_REG   = 0xB;
            public const int STATUS_REG    = 0xC;
            public const int CAUSE_REG     = 0xD;
            public const int EPC_REG       = 0xE;
            public const int PREVID_REG    = 0xF;
            public const int CONFIG_REG    = 0x10;
            public const int LLADDR_REG    = 0x11;
            public const int WATCHLO_REG   = 0x12;
            public const int WATCHHI_REG   = 0x13;
            public const int XCONTEXT_REG  = 0x14;
            public const int RESERVED1_REG = 0x15;
            public const int RESERVED2_REG = 0x16;
            public const int RESERVED3_REG = 0x17;
            public const int RESERVED4_REG = 0x18;
            public const int RESERVED5_REG = 0x19;
            public const int PERR_REG      = 0x1A;
            public const int CACHERR_REG   = 0x1B;
            public const int TAGLO_REG     = 0x1C;
            public const int TAGHI_REG     = 0x1D;
            public const int ERROREPC_REG  = 0x1E;
            public const int RESERVED6_REG = 0x1F;

            public static ulong[] Reg = new ulong[32];

            public static void PrintRegisterInfo()
            {
                for (int i = 0; i < Reg.Length; ++i)
                    Common.Logger.PrintInfoLine($"CP0R[{i}]: 0x{Reg[i]:x16}");
            }
        }

        public class COP1
        {
            public static double[] Reg = new double[32];

            public static void PrintRegisterInfo()
            {
                for (int i = 0; i < Reg.Length; ++i)
                    Common.Logger.PrintInfoLine($"CP1R[{i}]: {Reg[i]}");
            }
        }
    }
}
