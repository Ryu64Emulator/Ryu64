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

        private static uint GetCICSeed()
        {
            ulong CRC = 0;

            for (uint i = 0; i < 0xFC0; i += 4)
                CRC += memory.ReadUInt32(i + 0x10000040);

            switch (CRC)
            {
                default:
                case 0x000000D057C85244: // CIC_X102
                case 0x000000D0027FDF31: // CIC_X101
                case 0x000000CFFB631223: // CIC_X101
                    return 0x3F;
                case 0x000000D6497E414B: // CIC_X103
                    return 0x78;
                case 0x0000011A49F60E96: // CIC_X105
                    return 0x91;
                case 0x000000D6D5BE5580: // CIC_X106
                    return 0x85;
                case 0x000001053BC19870: // CIC_5167
                case 0x000000D2E53EF008: // CIC_8303
                case 0x000000D2E53EF39F: // CIC_DVDD
                    return 0xDD;
                case 0x000000D2E53E5DDA: // CIC_USDD
                    return 0xDE;
            }
        }

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

                Registers.R4300.PC = 0xBFC00000;
            }
            else
            {
                uint RomType   = 0; // 0 = Cart, 1 = DD
                uint ResetType = 0; // 0 = Cold Reset, 1 = NMI
                uint S7        = 0; // Unknown
                uint TVType    = 1; // 0 = PAL, 1 = NTSC, 2 = MPAL

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
                Registers.R4300.Reg[19] = RomType;
                Registers.R4300.Reg[20] = TVType;
                Registers.R4300.Reg[21] = ResetType;
                Registers.R4300.Reg[22] = GetCICSeed();
                Registers.R4300.Reg[23] = S7;
                Registers.R4300.Reg[25] = 0xFFFFFFFF9DEBB54F;
                Registers.R4300.Reg[29] = 0xFFFFFFFFA4001FF0;
                Registers.R4300.Reg[31] = 0xFFFFFFFFA4001550;
                Registers.R4300.HI      = 0x000000003FC18657;
                Registers.R4300.LO      = 0x000000003103E121;
                Registers.R4300.PC      = 0xA4000040;

                for (uint i = 0xB0000000, j = 0xA4000000; j < 0xA4000000 + 0x1000; ++i, ++j)
                    memory.WriteUInt8(j, memory.ReadUInt8(i)); // Load the Boot Code into the correct memory address
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
