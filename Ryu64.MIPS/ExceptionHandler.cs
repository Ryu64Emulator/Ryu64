using Ryu64.MIPS.Cores;

namespace Ryu64.MIPS
{
    public class ExceptionHandler
    {
        public enum ExcCode
        {
            Int = 0,
            Mod = 1 << 2,
            TLBL = 2 << 2,
            TLBS = 3 << 2,
            AdEL = 4 << 2,
            AdES = 5 << 2,
            IBE = 6 << 2,
            DBE = 7 << 2,
            Sys = 8 << 2,
            Bp = 9 << 2,
            RI = 10 << 2,
            CpU = 11 << 2,
            Ov = 12 << 2,
            Tr = 13 << 2,
            FPE = 15 << 2,
            WATCH = 23 << 2
        }

        public static void InvokeTLBMiss(uint AddressWrittenTo, bool Store)
        {
            uint VPN2 = (AddressWrittenTo / 0x1000) >> 1;
            Registers.COP0.Reg[Registers.COP0.BADVADDR_REG] = AddressWrittenTo;
            Registers.COP0.Reg[Registers.COP0.CONTEXT_REG]  = VPN2 << 4;
            Registers.COP0.Reg[Registers.COP0.XCONTEXT_REG] = VPN2 << 4;
            Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG]  = VPN2 << 13;

            Registers.COP0.Reg[Registers.COP0.EPC_REG]    = (R4300.isDelaySlot) ? Registers.R4300.PC : Registers.R4300.PC - 4;
            Registers.COP0.Reg[Registers.COP0.CAUSE_REG] |= (Store ? (uint)ExcCode.TLBS : (uint)ExcCode.TLBL) 
                                                            | ((R4300.isDelaySlot) ? (0x80000000) : 0);

            Common.Logger.PrintWarningLine($"TLB Miss at PC: 0x{Registers.R4300.PC:X8}, BadVAddr: 0x{AddressWrittenTo:X8}");

            if ((Registers.COP0.Reg[Registers.COP0.STATUS_REG] & 0b00000000010000000000000000000000) == 0b00000000010000000000000000000000)
                Registers.R4300.PC = 0xBFC00200;
            else
                Registers.R4300.PC = 0x80000000;
        }

        public static void InvokeBreak()
        {
            Registers.COP0.Reg[Registers.COP0.EPC_REG] = (R4300.isDelaySlot) ? Registers.R4300.PC : Registers.R4300.PC - 4;
            Registers.COP0.Reg[Registers.COP0.CAUSE_REG] |= (uint)ExcCode.Bp | ((R4300.isDelaySlot) ? (0x80000000) : 0);

            Common.Logger.PrintWarningLine($"Break at PC: 0x{Registers.R4300.PC:X8}");

            Registers.R4300.PC = 0x80000180;
        }

        public static void PollInt()
        {
            if ((uint)Registers.COP0.Reg[Registers.COP0.COUNT_REG] == (uint)Registers.COP0.Reg[Registers.COP0.COMPARE_REG])
                Registers.COP0.Reg[Registers.COP0.CAUSE_REG] |= 0x8000;

            if ((Registers.COP0.Reg[Registers.COP0.STATUS_REG] & 0b111) == 0b001)
            {
                uint Cause = (uint)Registers.COP0.Reg[Registers.COP0.CAUSE_REG] & 0xFF00;
                if (Cause > 0)
                {
                    uint Status = (uint)Registers.COP0.Reg[Registers.COP0.STATUS_REG] & 0xFF00;
                    if ((Cause & Status) > 0)
                    {
                        Registers.COP0.Reg[Registers.COP0.EPC_REG] = (R4300.isDelaySlot) ? Registers.R4300.PC : Registers.R4300.PC - 4;
                        Registers.COP0.Reg[Registers.COP0.CAUSE_REG] |= (uint)ExcCode.Int | ((R4300.isDelaySlot) ? (0x80000000) : 0);

                        Common.Logger.PrintInfoLine($"Interrupt at PC: 0x{Registers.R4300.PC:X8}");

                        Registers.R4300.PC = 0x80000180;
                    }
                }
            }
        }
    }
}
