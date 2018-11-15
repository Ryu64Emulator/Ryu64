namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void TLBWI(OpcodeTable.OpcodeDesc Desc)
        {
            TLB.WriteTLBEntryIndexed();
            Registers.R4300.PC += 4;
        }
    }
}
