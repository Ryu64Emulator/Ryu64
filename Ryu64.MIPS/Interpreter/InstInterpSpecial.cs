using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public static void SYSCALL(OpcodeTable.OpcodeDesc Desc)
        {
            if (Desc.op4 > 0 && Common.Settings.SUPPORT_CPUTEST_SYSCALL)
            {
                // Special type of SYSCALL used for the Fraser CPU Tests
                char TestChar = (char)(Desc.op2 + 64);
                string TestResult = $"Test Result - Set:{Desc.op3} Test:{Desc.op4} Result:{(char)(Desc.op2 + 64)}";

                if ((char)(Desc.op2 + 64) == 'F')
                    Common.Logger.PrintErrorLine(TestResult);
                else if ((char)(Desc.op2 + 64) == 'P')
                    Common.Logger.PrintSuccessLine(TestResult);
                else
                    Common.Logger.PrintInfoLine(TestResult);
            }
            else
            {
                // Regular SYSCALL behavior
            }

            Registers.R4300.PC += 4;
        }

        public static void BREAK(OpcodeTable.OpcodeDesc Desc)
        {
            Common.Util.Cleanup(Registers.R4300.PC);
            throw new Common.Exceptions.ProgramBreakPointException("Guest broke execution.");
        }
    }
}
