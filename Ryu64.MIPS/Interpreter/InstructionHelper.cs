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
                throw new NotImplementedException("The RSP is not implemented yet.");

            throw new ArgumentException("While executing a Delay Slot RSP nor CPU was true.");
        }
    }
}
