namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void MFC0(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.Reg[Desc.op2] = (uint)Registers.COP0.Reg[Desc.op3];
            Registers.R4300.PC += 4;
        }

        public static void MTC0(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.COP0.Reg[Desc.op3] = (uint)Registers.R4300.Reg[Desc.op2];
            Registers.R4300.PC += 4;
        }

        public static void CACHE(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4; // Stubbed.
        }
    }
}
