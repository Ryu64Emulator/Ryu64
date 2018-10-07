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

                Registers.R4300.PC = 0xBFC00000;
            }
            else
            {
                Registers.R4300.Reg[1]  = 0x0000000000000001;
                Registers.R4300.Reg[2]  = 0x000000000EBDA536;
                Registers.R4300.Reg[3]  = 0x000000000EBDA536;
                Registers.R4300.Reg[4]  = 0x000000000000A536;
                Registers.R4300.Reg[5]  = 0xFFFFFFFFC0F1D859;
                Registers.R4300.Reg[6]  = 0xFFFFFFFFA4001F0C;
                Registers.R4300.Reg[7]  = 0xFFFFFFFFA4001F08;
                Registers.R4300.Reg[8]  = 0x00000000000000C0;
                Registers.R4300.Reg[10] = 0x0000000000000040;
                Registers.R4300.Reg[11] = 0xFFFFFFFFA4000040;
                Registers.R4300.Reg[12] = 0xFFFFFFFFED10D0B3;
                Registers.R4300.Reg[13] = 0x000000001402A4CC;
                Registers.R4300.Reg[14] = 0x000000002DE108EA;
                Registers.R4300.Reg[15] = 0x000000003103E121;
                Registers.R4300.Reg[20] = 0x0000000000000001;
                Registers.R4300.Reg[22] = 0x000000000000003F;
                Registers.R4300.Reg[25] = 0xFFFFFFFF9DEBB54F;
                Registers.R4300.Reg[29] = 0xFFFFFFFFA4001FF0;
                Registers.R4300.Reg[31] = 0xFFFFFFFFA4001550;
                Registers.R4300.HI      = 0x000000003FC18657;
                Registers.R4300.LO      = 0x000000003103E121;
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

                    while (Common.Settings.STEP_MODE && !Common.Variables.Step);
                    if (Common.Settings.STEP_MODE)
                    {
                        Registers.R4300.PrintRegisterInfo();
                        Registers.COP0.PrintRegisterInfo();
                        Registers.COP1.PrintRegisterInfo();
                        Thread.Sleep(250);
                        Common.Variables.Step = false;
                    }
                }
                if (Common.Settings.MEASURE_SPEED) Common.Measure.MeasureTime.Stop();
            });

            CPUThread.Start();
        }
    }
}
