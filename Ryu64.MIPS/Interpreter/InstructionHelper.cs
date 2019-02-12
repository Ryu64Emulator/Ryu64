using System;
using Ryu64.MIPS.Cores;

namespace Ryu64.MIPS.Interpreter
{
    public class InstructionHelper
    {
        public static void ExecuteDelaySlot(bool RSP, bool CPU)
        {
            if (CPU)
            {
                R4300.InterpretOpcode(R4300.memory.ReadUInt32(Registers.R4300.PC), true);
                return;
            }
            else if (RSP)
            {
                Cores.RSP.InterpretOpcode(R4300.memory.ReadIMEMInstruction(Registers.RSPReg.PC));
                return;
            }

            throw new ArgumentException("While executing a Delay Slot RSP nor CPU was true.");
        }
    }
}
