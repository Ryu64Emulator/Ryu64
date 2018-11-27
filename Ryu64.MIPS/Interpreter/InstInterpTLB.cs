namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void TLBR(OpcodeTable.OpcodeDesc Desc)
        {
            TLB.ReadTLBEntry();
            Registers.R4300.PC += 4;
        }

        public static void TLBWI(OpcodeTable.OpcodeDesc Desc)
        {
            TLB.WriteTLBEntryIndexed();
            Registers.R4300.PC += 4;
        }

        public static void TLBWR(OpcodeTable.OpcodeDesc Desc)
        {
            TLB.WriteTLBEntryRandom();
            Registers.R4300.PC += 4;
        }

        public static void TLBP(OpcodeTable.OpcodeDesc Desc)
        {
            TLB.ProbeTLB();
            Registers.R4300.PC += 4;
        }
    }
}
