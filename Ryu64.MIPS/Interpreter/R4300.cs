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

        public static Memory memory;

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
                    Desc.Imm, Desc.Target);
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

                for (uint i = 0x0, j = 0x1FC00000; j < 0x1FC007BF; ++i, ++j)
                    memory.WriteUInt8(j, PIF[i]); // Load the PIF rom into memory

                /*
                memory.WriteUInt32(0x1FC001EC,      0x3c0ba400);
                memory.WriteUInt32(0x1FC001EC + 4,  0x256b1630);
                memory.WriteUInt32(0x1FC001EC + 8,  0x01600008);
                memory.WriteUInt32(0x1FC001EC + 12, 0x00000000);
                */

                Registers.R4300.PC = 0x1FC00000;
            }
            else
            {
                Registers.R4300.Reg[20] = 0x1;
                Registers.R4300.Reg[22] = 0x3F;
                Registers.R4300.Reg[29] = 0xA4001FF0;
                Registers.R4300.PC      = 0xA4000040;

                memory.WriteUInt32(0x04300004, 0x01010101);

                for (uint i = 0xB0000000, j = 0xA4000000; j < 0xA4000000 + 0x1000; ++i, ++j)
                    memory.WriteUInt8(j, memory.ReadUInt8(i)); // Load the rom into the correct memory address
            }

            COP0.PowerOnCOP0();
            COP1.PowerOnCOP1();

            R4300_ON = true;

            OpcodeTable.Init();

            if (Common.Settings.DUMP_MEMORY)
            {
                string Output = "";
                for (uint i = Common.Settings.DUMP_MEMORY_START; i <= Common.Settings.DUMP_MEMORY_END; i += 4)
                {
                    uint Current = 0;

                    string Line;

                    try
                    {
                        Current = memory.ReadUInt32(i);
                    }
                    catch
                    {
                        Line = "NONE\n";
                        Output += Line;
                        Common.Logger.PrintInfo(Line);
                        continue;
                    }

                    OpcodeTable.OpcodeDesc Desc = new OpcodeTable.OpcodeDesc(Current);
                    OpcodeTable.InstInfo   Info = new OpcodeTable.InstInfo();

                    try
                    {
                        Info = OpcodeTable.GetOpcodeInfo(Current);
                    }
                    catch
                    {
                        Line = $"0x{i:x8}: 0x{Current:x8}: UNK\n";
                        Output += Line;
                        Common.Logger.PrintInfo(Line);
                        continue;
                    }

                    string ASM = string.Format(
                        Info.FormattedASM,
                        Desc.op1, Desc.op2, Desc.op3, Desc.op4,
                        Desc.Imm, Desc.Target);
                    Line = $"0x{i:x8}: 0x{Current:x8}: {ASM}\n";
                    Output += Line;
                    Common.Logger.PrintInfo(Line);
                }

                File.WriteAllText("./DMP_MEM", Output);
            }

            Thread CPUThread = 
            new Thread(() => 
            {
                if (Common.Settings.MEASURE_SPEED) Common.Measure.MeasureTime.Start();
                while (R4300_ON)
                {
                    uint Opcode = memory.ReadUInt32(Registers.R4300.PC);
                    InterpretOpcode(Opcode);
                }
                if (Common.Settings.MEASURE_SPEED) Common.Measure.MeasureTime.Stop();
            });

            CPUThread.Start();
        }
    }
}
