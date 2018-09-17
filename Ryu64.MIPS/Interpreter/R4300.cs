using System;
using System.Threading;

namespace Ryu64.MIPS
{
    public class R4300
    {
        public static bool R4300_ON = false;

        public static void InterpretOpcode(uint Opcode)
        {
            OpcodeTable.GetInterpreterMethod(Opcode)(new OpcodeTable.OpcodeDesc(Opcode));
            Registers.R4300.PC += 4;
        }

        public static void PowerOnR4300(ulong PC)
        {
            for (int i = 0; i < Registers.R4300.Reg.Length; ++i)
                Registers.R4300.Reg[i] = 0; // Clear Registers

            R4300_ON = true;

            COP0.PowerOnCOP0();
            COP1.PowerOnCOP1();

            OpcodeTable.Init();

            Registers.R4300.PC = 0x0;

            Thread CPUThread = 
            new Thread(() => 
            {
                while (R4300_ON)
                {
                    uint Opcode = Memory.ReadUInt32(Registers.R4300.PC);
                    Console.WriteLine($"0x{Registers.R4300.PC:x}: {Convert.ToString(Opcode, 2)}");
                    InterpretOpcode(Opcode);
                }
            });

            CPUThread.Start();
        }
    }
}
