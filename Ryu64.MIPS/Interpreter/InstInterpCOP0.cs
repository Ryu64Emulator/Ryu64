namespace Ryu64.MIPS.Interpreter
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

        public static void ERET(OpcodeTable.OpcodeDesc Desc)
        {
            if ((Registers.COP0.Reg[Registers.COP0.STATUS_REG] & 0b100) == 0b100)
            {
                Registers.R4300.PC = (uint)Registers.COP0.Reg[Registers.COP0.ERROREPC_REG];
                Registers.COP0.Reg[Registers.COP0.STATUS_REG] &= ~(uint)0b100;
            }
            else
            {
                Registers.R4300.PC = (uint)Registers.COP0.Reg[Registers.COP0.EPC_REG];
                Registers.COP0.Reg[Registers.COP0.STATUS_REG] &= ~(uint)0b010;
            }
            Registers.R4300.LLbit = 0;
        }
    }
}
