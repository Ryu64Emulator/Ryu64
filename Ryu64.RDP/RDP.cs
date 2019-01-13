using System;
using System.Threading;

namespace Ryu64.RDP
{
    public class RDP
    {
        public static uint   ColorImageAddr   = 0x0;
        public static byte   ColorImageFormat = 0;
        public static byte   ColorImageSize   = 0;
        public static ushort ColorImageWidth  = 0;

        public static uint FillColor = 0;

        public static void PowerOnRDP()
        {
            Thread RDPThread = new Thread(() =>
            {
                while (MIPS.R4300.R4300_ON)
                {
                    if (MIPS.RDPWrapper.RDPExec)
                    {
                        uint PC = MIPS.R4300.memory.ReadUInt32(0x04100000);
                        if (Common.Variables.Debug)
                            Common.Logger.PrintInfoLine($"RDP | Starting Command Buffer, 0x{MIPS.R4300.memory.ReadUInt32(0x04100000):x8} to 0x{MIPS.R4300.memory.ReadUInt32(0x04100004):x8}");
                        while (PC < MIPS.R4300.memory.ReadUInt32(0x04100004))
                        {
                            ulong Instruction = MIPS.R4300.memory.ReadUInt64(PC);
                            byte Command = (byte)(((Instruction & 0xFF00000000000000) >> 56) & 0xFF);
                            if (Common.Variables.Debug)
                                Common.Logger.PrintInfoLine($"RDP | 0x{PC:x8}: 0x{Instruction:x16}");
                            switch (Command)
                            {
                                case 0x3F: // Set Color Image
                                    byte   Format = (byte)  ((Instruction & 0x00E0000000000000) >> 53);
                                    byte   Size   = (byte)  ((Instruction & 0x0018000000000000) >> 51);
                                    ushort Width  = (ushort)((Instruction & 0x000003FF00000000) >> 32);
                                    uint   Addr   = (uint)   (Instruction & 0x000000001FFFFFFF);

                                    ColorImageAddr   = Addr;
                                    ColorImageFormat = Format;
                                    ColorImageSize   = Size;
                                    ColorImageWidth  = Width;

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Set Color Image - Address: 0x{ColorImageAddr:x8}, Format: {ColorImageFormat}, Size: {ColorImageSize}, Image Width: {ColorImageWidth + 1}");

                                    PC += 8;
                                    break;
                                case 0x2D: // Set Scissor
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x2F: // Set Other Modes
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x29: // Sync Full
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x31: // Sync Load
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x37: // Set Fill Color
                                    FillColor = (uint)(Instruction & 0xFFFFFFFF);

                                    PC += 8;
                                    break;
                                case 0x36: // Fill Rectangle
                                    ushort XL = (ushort)((Instruction & 0x00FFF00000000000) >> 44);
                                    ushort YL = (ushort)((Instruction & 0x00000FFF00000000) >> 32);
                                    ushort XH = (ushort)((Instruction & 0x0000000000FFF000) >> 12);
                                    ushort YH = (ushort) (Instruction & 0x0000000000000FFF);

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Fill Rectangle - XL: {XL >> 2} (10.2 Format), YL: {YL >> 2} (10.2 Format), XH: {XH >> 2} (10.2 Format), YH: {YH >> 2} (10.2 Format)");

                                    FillRect(XL, YL, XH, YH);
                                    PC += 8;
                                    break;
                                case 0x00: // No op
                                    PC += 8;
                                    break;
                                default:
                                    throw new NotImplementedException($"Command 0x{Command:x2} is not an implemented RDP command.  PC: 0x{PC:x8}");
                            }
                        }
                        MIPS.RDPWrapper.RDPExec = false;
                    }
                    Thread.Sleep(1);
                }
            })
            {
                Name = "RDP"
            };
            RDPThread.Start();
        }

        private static void FillRect(ushort XL, ushort YL, ushort XH, ushort YH)
        {
            if (ColorImageFormat == 0)
            {
                uint ImageWidth = ColorImageWidth + (uint)1;

                uint RectX1 = (uint)(XH >> 2);
                uint RectY1 = (uint)(YH >> 2);
                uint RectX2 = (uint)(XL >> 2);
                uint RectY2 = (uint)(YL >> 2);

                if (ColorImageSize == 3)
                {
                    for (uint y = RectY1; y < RectY2; ++y)
                    {
                        for (uint x = RectX1; x < RectX2; ++x)
                        {
                            uint Index = x + y * ImageWidth;
                            MIPS.R4300.memory.WriteUInt32(ColorImageAddr + (Index * 4), FillColor);
                        }
                    }
                }
                else if (ColorImageSize == 2)
                {
                    for (uint y = RectY1; y < RectY2; ++y)
                    {
                        for (uint x = RectX1; x < RectX2; x += 2)
                        {
                            uint Index = x + y * ImageWidth;
                            MIPS.R4300.memory.WriteUInt32(ColorImageAddr + (Index * 2), FillColor);
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException($"Pixel Size: {ColorImageSize} is not supported with Rectangles yet.");
                }
            }
            else
            {
                throw new NotImplementedException($"Rectangles aren't supported in any other mode other than RGBA currently.");
            }
        }
    }
}
