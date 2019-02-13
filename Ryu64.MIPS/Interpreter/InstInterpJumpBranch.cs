namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void BEQ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) == (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BEQL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            if ((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) == (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU))
            {
                InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
            }
            else Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void BGEZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if ((int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) >= 0)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BGEZAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) >= 0;
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetMainReg(31, Registers.ReadPC(Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BGTZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) > 0;
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BLEZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) <= 0;
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BLEZL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            if ((int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) <= 0)
            {
                InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
            }
            else Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void BLTZ(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) <= 0;
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BLTZAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (int)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) <= 0;
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetMainReg(31, Registers.ReadPC(Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BNE(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            bool Cond = (uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) != (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            if (Cond)
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
        }

        public static void BNEL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            if ((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU) != (uint)Registers.ReadMainReg(Desc.op2, Desc.RSP, Desc.CPU))
            {
                InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
                Registers.SetPC((uint)((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) + (short)(Desc.Imm << 2)), Desc.RSP, Desc.CPU);
            }
            else Registers.AddPC(4, Desc.RSP, Desc.CPU);
        }

        public static void J(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetPC(((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) & 0xF0000000) | (Desc.Target << 2), Desc.RSP, Desc.CPU);
        }

        public static void JAL(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetMainReg(31, Registers.ReadPC(Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.SetPC(((Registers.ReadPC(Desc.RSP, Desc.CPU) - 4) & 0xF0000000) | (Desc.Target << 2), Desc.RSP, Desc.CPU);
        }

        public static void JALR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetMainReg(Desc.op3, Registers.ReadPC(Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
            Registers.SetPC((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
        }

        public static void JR(OpcodeTable.OpcodeDesc Desc)
        {
            Registers.AddPC(4, Desc.RSP, Desc.CPU);
            InstructionHelper.ExecuteDelaySlot(Desc.RSP, Desc.CPU);
            Registers.SetPC((uint)Registers.ReadMainReg(Desc.op1, Desc.RSP, Desc.CPU), Desc.RSP, Desc.CPU);
        }
    }
}
