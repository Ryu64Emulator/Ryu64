using System;

namespace Ryu64.MIPS.Interpreter
{
    public partial class InstInterp
    {
        public static void SYSCALL(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op4 > 0 && Common.Variables.UTEsyscall)
            {
                // Special type of SYSCALL used for the Fraser CPU Tests
                char TestChar = (char)(Desc.op2 + 64);
                string TestResult = $"Test Result - Set:{Desc.op3} Test:{Desc.op4} Result:{TestChar}";

                if (TestChar == 'F')
                    Common.Logger.PrintErrorLine(TestResult);
                else if (TestChar == 'P')
                    Common.Logger.PrintSuccessLine(TestResult);
                else
                    Common.Logger.PrintInfoLine(TestResult);
            }
            else
            {
                // Regular SYSCALL behavior
                throw new NotImplementedException("Regular SYSCALL behavior is not implemented.");
            }

            Registers.R4300.PC += 4;
        }

        public static void BREAK(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.CPU)
            {
                ExceptionHandler.InvokeBreak();
                return;
            }

            Cores.R4300.memory.WriteUInt32(0x04040010, 2); // SP_STATUS_REG
            Cores.R4300.memory.SetRSPBroke();

            if ((Cores.R4300.memory.ReadUInt32(0x04040010) & 0x40) > 0)
                MI.InvokeSPInterrupt();
        }
    }
}
