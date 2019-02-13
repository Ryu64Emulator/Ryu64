using System;

namespace Ryu64.MIPS
{
    public class Registers
    {
        public static ulong ReadMainReg(byte Index, bool RSP, bool CPU)
        {
            if (CPU)
                return R4300.Reg[Index];
            else if (RSP)
                return RSPReg.Reg[Index];

            throw new ArgumentException("When reading a Main Register RSP nor CPU was true.");
        }

        public static ulong ReadCop0Reg(byte Index, bool RSP, bool CPU)
        {
            if (CPU)
                return COP0.Reg[Index];
            else if (RSP)
                return RSPCOP0.Reg[Index];

            throw new ArgumentException("When reading a COP0 Register RSP nor CPU was true.");
        }

        public static void SetMainReg(byte Index, ulong Value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                R4300.Reg[Index] = Value;
                return;
            }
            else if (RSP)
            {
                RSPReg.Reg[Index] = (uint)Value;
                return;
            }

            throw new ArgumentException("When setting a Main Register RSP nor CPU was true.");
        }

        public static void SetCop0Reg(byte Index, ulong Value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                COP0.Reg[Index] = Value;
                return;
            }
            else if (RSP)
            {
                RSPCOP0.Reg[Index] = (uint)Value;
                return;
            }

            throw new ArgumentException("When setting a COP0 Register RSP nor CPU was true.");
        }

        public static uint ReadPC(bool RSP, bool CPU)
        {
            if (CPU)
                return R4300.PC;
            else if (RSP)
                return RSPReg.PC;

            throw new ArgumentException("When reading the PC RSP nor CPU was true.");
        }

        public static void SetPC(uint Value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                R4300.PC = Value;
                return;
            }
            else if (RSP)
            {
                RSPReg.PC = (ushort)Value;
                return;
            }

            throw new ArgumentException("When setting the PC RSP nor CPU was true.");
        }

        public static void AddPC(uint Add, bool RSP, bool CPU)
        {
            if (CPU)
            {
                R4300.PC += Add;
                return;
            }
            else if (RSP)
            {
                RSPReg.PC += (ushort)Add;
                return;
            }

            throw new ArgumentException("When adding to the PC RSP nor CPU was true.");
        }

        public class R4300
        {
            public static ulong[] Reg = new ulong[32];
            public static ulong HI;
            public static ulong LO;
            public static uint  PC;
            public static byte  LLbit;
        }

        public class COP0
        {
            public static bool FR_STATUS
            {
                get
                {
                    return (Reg[STATUS_REG] & 0x4000000) > 0;
                }
            }

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
        }

        public class RSPReg
        {
            public static uint[] Reg = new uint[32];
            public static ushort PC
            {
                get
                {
                    return (ushort)(Cores.R4300.memory.ReadUInt32(0x04080000) & 0xFFF);
                }
                set
                {
                    Cores.R4300.memory.WriteUInt32(0x04080000, (uint)value & 0xFFF);
                }
            }
        }

        public class RSPCOP0
        {
            public static uint[] Reg = new uint[16];
        }

        public class RSPCOP2
        {
            public static ushort  VCC;
            public static ushort  VCO;
            public static byte    VCE;
            public static ulong[] ACC = new ulong[8];
            public static VectorRegister[] Reg = new VectorRegister[32];
        }

        public class COP1
        {
            public static ulong[] Reg = new ulong[32];
        }
    }
}
