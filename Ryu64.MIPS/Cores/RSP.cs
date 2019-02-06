namespace Ryu64.MIPS.Cores
{
    public class RSP
    {
        public static bool RSP_ON   = false;
        public static bool RSP_IDLE = true;

        public static void InterpretOpcode(uint Opcode)
        {
            OpcodeTable.OpcodeDesc Desc = new OpcodeTable.OpcodeDesc(Opcode, true, false);
            OpcodeTable.InstInfo   Info = OpcodeTable.GetOpcodeInfo (Opcode, true, false);

            Info.Interpret(Desc);
        }

        public static void PowerOnRSP()
        {

        }
    }
}
