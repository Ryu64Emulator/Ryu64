namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void NOP(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU); // The NOP Instruction, do nothing.
        }
    }
}
