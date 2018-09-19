using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ryu64.MIPS
{
    public class R4300
    {
        public static bool R4300_ON = false;

        public static void InterpretOpcode(uint Opcode)
        {
            if (Registers.R4300.Reg[0] != 0) Registers.R4300.Reg[0] = 0;

            OpcodeTable.OpcodeDesc Desc = new OpcodeTable.OpcodeDesc(Opcode);
            OpcodeTable.InstInfo   Info = OpcodeTable.GetOpcodeInfo(Opcode);

            if (Common.Settings.DEBUG)
            {
                string ASM = string.Format(
                    Info.FormattedASM,
                    Desc.op1, Desc.op2, Desc.op3, Desc.op4,
                    Desc.Imm, Desc.Target,
                    Desc.ExceptionCode20bit, Desc.ExceptionCode10bit);
                Common.Logger.PrintInfoLine($"0x{Registers.R4300.PC:x}: {Convert.ToString(Opcode, 2).PadLeft(32, '0')}: {ASM}");
            }

            Info.Interpret(Desc);
            if (Common.Settings.MEASURE_SPEED) Common.Measure.InstructionCount += 1;
        }

        public static void PowerOnR4300(ulong PC)
        {
            for (int i = 0; i < Registers.R4300.Reg.Length; ++i)
                Registers.R4300.Reg[i] = 0; // Clear Registers

            if (Common.Settings.LOAD_PIF)
            {
                byte[] PIF = File.ReadAllBytes(Common.Settings.PIF_ROM);

                for (ulong i = 0x0, j = 0x1FC00000; j < 0x1FC007BF; ++i, ++j)
                    Memory.WriteUInt8(j, PIF[i]); // Load the PIF rom into memory

                Registers.R4300.PC = 0x1FC00000;
            }
            else
            {
                Registers.R4300.Reg[20] = 0x1;
                Registers.R4300.Reg[22] = 0x3F;
                Registers.R4300.Reg[29] = 0xA4001FF0;
                Registers.R4300.PC      = 0xA4000040;

                Memory.WriteUInt32(0x04300004, 0x01010101);

                for (ulong i = 0xB0000000, j = 0xA4000000; j < 0xA4000000 + 0x1000; ++i, ++j)
                    Memory.WriteUInt8(j, Memory.ReadUInt8(i)); // Load the rom into the correct memory address
            }

            COP0.PowerOnCOP0();
            COP1.PowerOnCOP1();

            R4300_ON = true;

            OpcodeTable.Init();

            Thread CPUThread = 
            new Thread(() => 
            {
                if (Common.Settings.MEASURE_SPEED) Common.Measure.MeasureTime.Start();
                while (R4300_ON)
                {
                    uint Opcode = Memory.ReadUInt32(Registers.R4300.PC);
                    InterpretOpcode(Opcode);
                }
                if (Common.Settings.MEASURE_SPEED) Common.Measure.MeasureTime.Stop();
            });

            CPUThread.Start();
        }
    }
}
