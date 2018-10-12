namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void NOP(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.R4300.PC += 4; // The NOP Instruction, do nothing.
        }
    }
}
