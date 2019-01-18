using System;
using System.Threading;

namespace Ryu64.RDP
{
    public class RDP
    {
        private static IRasterizer Rasterizer;

        // Color Image Variables
        public static uint   ColorImageAddr   = 0x0;
        public static byte   ColorImageFormat = 0;
        public static byte   ColorImageSize   = 0;
        public static ushort ColorImageWidth  = 0;

        // Texture Image Variables
        public static uint   TexImageAddr   = 0x0;
        public static byte   TexImageFormat = 0;
        public static byte   TexImageSize   = 0;
        public static ushort TexImageWidth  = 0;

        // Scissor Border Variables
        public static ushort ScissorXH = 0;
        public static ushort ScissorYH = 0;
        public static ushort ScissorXL = 0;
        public static ushort ScissorYL = 0;

        // Current Fill Color
        public static uint FillColor = 0;

        public enum ImageFormat
        {
            RGBA = 0,
            YUV = 1,
            ColorIndx = 3,
            IA = 3,
            I = 4
        }

        public enum BPP
        {
            BPP4 = 0,
            BPP8 = 1,
            BPP16 = 2,
            BPP32 = 3,
            Other = 4
        }

        public struct Tile
        {
            public ImageFormat format;
            public BPP Size;
            public ushort Line;
            public ushort TmemAddr;
            public byte Palette;

            public byte CT;
            public byte MT;
            public byte MaskT;
            public byte ShiftT;

            public byte CS;
            public byte MS;
            public byte MaskS;
            public byte ShiftS;
        }

        public static Tile[] Tiles;

        public static void PowerOnRDP()
        {
            Tiles = new Tile[8];

            Rasterizer = new SoftwareRasterizer();

            Thread RDPThread = new Thread(() =>
            {
                while (MIPS.R4300.R4300_ON)
                {
                    if (MIPS.RDPWrapper.RDPExec)
                    {
                        uint PC = MIPS.R4300.memory.ReadUInt32(0x04100000);
                        if (Common.Variables.Debug)
                            Common.Logger.PrintInfoLine($"RDP | Starting Command Buffer, 0x{MIPS.R4300.memory.ReadUInt32(0x04100000):x8} to 0x{MIPS.R4300.memory.ReadUInt32(0x04100004):x8}.");
                        while (PC < MIPS.R4300.memory.ReadUInt32(0x04100004))
                        {
                            ulong Instruction = MIPS.R4300.memory.ReadUInt64(PC);
                            byte Command = (byte)(((Instruction & 0xFF00000000000000) >> 56) & 0xFF);
                            if (Common.Variables.Debug)
                                Common.Logger.PrintInfoLine($"RDP | 0x{PC:x8}: 0x{Instruction:x16}");
                            switch (Command)
                            {
                                case 0x3F: // Set Color Image
                                    byte   SCI_Format = (byte)  ((Instruction & 0x00E0000000000000) >> 53);
                                    byte   SCI_Size   = (byte)  ((Instruction & 0x0018000000000000) >> 51);
                                    ushort SCI_Width  = (ushort)((Instruction & 0x000003FF00000000) >> 32);
                                    uint   SCI_Addr   = (uint)   (Instruction & 0x000000001FFFFFFF);

                                    ColorImageAddr   = SCI_Addr;
                                    ColorImageFormat = SCI_Format;
                                    ColorImageSize   = SCI_Size;
                                    ColorImageWidth  = SCI_Width;

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Set Color Image - Address: 0x{ColorImageAddr:x8}, Format: {ColorImageFormat}, Size: {ColorImageSize}, Image Width: {ColorImageWidth + 1}");

                                    PC += 8;
                                    break;
                                case 0x3D: // Set Texture Image
                                    byte   STI_Format = (byte)  ((Instruction & 0x00E0000000000000) >> 53);
                                    byte   STI_Size   = (byte)  ((Instruction & 0x0018000000000000) >> 51);
                                    ushort STI_Width  = (ushort)((Instruction & 0x000003FF00000000) >> 32);
                                    uint   STI_Addr   = (uint)   (Instruction & 0x000000001FFFFFFF);

                                    TexImageAddr   = STI_Addr;
                                    TexImageFormat = STI_Format;
                                    TexImageSize   = STI_Size;
                                    TexImageWidth  = STI_Width;

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Set Texture Image - Address: 0x{TexImageAddr:x8}, Format: {TexImageFormat}, Size: {TexImageSize}, Image Width: {TexImageWidth + 1}");

                                    PC += 8;
                                    break;
                                case 0x2D: // Set Scissor
                                    ushort SS_XH = (ushort)((Instruction & 0x00FFF00000000000) >> 44);
                                    ushort SS_YH = (ushort)((Instruction & 0x00000FFF00000000) >> 32);
                                    ushort SS_XL = (ushort)((Instruction & 0x0000000000FFF000) >> 12);
                                    ushort SS_YL = (ushort) (Instruction & 0x0000000000000FFF);

                                    ScissorXH = SS_XH;
                                    ScissorYH = SS_YH;
                                    ScissorXL = SS_XL;
                                    ScissorYL = SS_YL;

                                    PC += 8;
                                    break;
                                case 0x2F: // Set Other Modes
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x29: // Sync Full
                                    MIPS.MI.InvokeDPInterrupt();
                                    PC += 8;
                                    break;
                                case 0x31: // Sync Load
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x27: // Sync Pipe
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x37: // Set Fill Color
                                    FillColor = (uint)(Instruction & 0xFFFFFFFF);

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Set Fill Color - Color: 0x{FillColor:x8}");

                                    PC += 8;
                                    break;
                                case 0x35: // Set Tile
                                    // Stubbed.
                                    PC += 8;
                                    break;
                                case 0x36: // Fill Rectangle
                                    ushort FR_XL = (ushort)((Instruction & 0x00FFF00000000000) >> 44);
                                    ushort FR_YL = (ushort)((Instruction & 0x00000FFF00000000) >> 32);
                                    ushort FR_XH = (ushort)((Instruction & 0x0000000000FFF000) >> 12);
                                    ushort FR_YH = (ushort) (Instruction & 0x0000000000000FFF);

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Fill Rectangle - XL: {FR_XL >> 2}.{FR_XL & 3}, YL: {FR_YL >> 2}.{FR_YL & 3}, XH: {FR_XH >> 2}.{FR_XH & 3}, YH: {FR_YH >> 2}.{FR_YH & 3}");

                                    Rasterizer.FillRect(FR_XL, FR_YL, FR_XH, FR_YH);

                                    PC += 8;
                                    break;
                                case 0x08: // Non-Shaded Triangle
                                    byte   NST_Dir = (byte)  ((Instruction & 0x0080000000000000) >> 55);
                                    ushort NST_YL  = (ushort)((Instruction & 0x00003FFF00000000) >> 32);
                                    ushort NST_YM  = (ushort)((Instruction & 0x000000003FFF0000) >> 16);
                                    ushort NST_YH  = (ushort) (Instruction & 0x0000000000003FFF);

                                    ulong NST_EdgeCoefficient1 = MIPS.R4300.memory.ReadUInt64(PC + 8);
                                    ulong NST_EdgeCoefficient2 = MIPS.R4300.memory.ReadUInt64(PC + 16);
                                    ulong NST_EdgeCoefficient3 = MIPS.R4300.memory.ReadUInt64(PC + 24);

                                    uint NST_XL    = (uint)((NST_EdgeCoefficient1 & 0xFFFFFFFF00000000) >> 32);
                                    uint NST_DxLDy = (uint) (NST_EdgeCoefficient1 & 0x00000000FFFFFFFF);

                                    uint NST_XH    = (uint)((NST_EdgeCoefficient2 & 0xFFFFFFFF00000000) >> 32);
                                    uint NST_DxHDy = (uint) (NST_EdgeCoefficient2 & 0x00000000FFFFFFFF);

                                    uint NST_XM    = (uint)((NST_EdgeCoefficient3 & 0xFFFFFFFF00000000) >> 32);
                                    uint NST_DxMDy = (uint) (NST_EdgeCoefficient3 & 0x00000000FFFFFFFF);

                                    if (Common.Variables.Debug)
                                        Common.Logger.PrintInfoLine($"RDP | Fill Triangle - YL: {NST_YL >> 2}, YM: {NST_YM >> 2}, YH: {NST_YH}, XL: {NST_XL >> 16}, DxLDy: {(short)(NST_DxLDy >> 16) + ((float)(NST_DxLDy & 0xFFFF) / 65535)}, XH: {NST_XH >> 16}, DxHDy: {(short)(NST_DxHDy >> 16) + ((float)(NST_DxHDy & 0xFFFF) / 65535)}, XM: {NST_XM >> 16}, DxMDy: {(short)(NST_DxMDy >> 16) + ((float)(NST_DxMDy & 0xFFFF) / 65535)}, Dir: {NST_Dir}");

                                    Rasterizer.FillTriangle(NST_YL, NST_YM, NST_YH, NST_XL, NST_DxLDy, NST_XH, NST_DxHDy, NST_XM, NST_DxMDy, NST_Dir);

                                    PC += 32;
                                    break;
                                case 0x00: // No op
                                    PC += 8;
                                    break;
                                default:
                                    throw new NotImplementedException($"Command 0x{Command:x2} is not an implemented RDP command.  PC: 0x{PC:x8}");
                            }

                            MIPS.R4300.memory.WriteRDPPC(PC);
                        }
                        if (Common.Variables.Debug)
                            Common.Logger.PrintInfoLine("RDP | Ended command buffer.");
                        MIPS.RDPWrapper.RDPExec = false;
                        MIPS.R4300.memory.WriteRDPPC(0);
                    }
                    Thread.Sleep(1);
                }
            })
            {
                Name = "RDP"
            };
            RDPThread.Start();
        }
    }
}
