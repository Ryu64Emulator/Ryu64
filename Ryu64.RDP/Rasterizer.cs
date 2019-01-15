using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.RDP
{
    public class Rasterizer
    {
        public static void SetPixel(uint x, uint y, uint Color, uint PixelSize, uint SX1, uint SY1, uint SX2, uint SY2, uint ImageWidth)
        {
            if ((x < SX1 || y < SY1) || (x > SX2 || y > SY2)) return;

            uint Index = x + y * ImageWidth;
            MIPS.R4300.memory.WriteUInt32(RDP.ColorImageAddr + (Index * PixelSize), Color);
        }

        private static void DrawFillScanBuffer(uint[] Scanbuffer)
        {
            if (RDP.ColorImageFormat == (uint)RDP.ImageFormat.RGBA)
            {
                uint ImageWidth = RDP.ColorImageWidth + (uint)1;

                // Pre Calculate the Screen Borders using the 10.2 format Scissor variables.
                uint ScreenX1 = (uint)(RDP.ScissorXH >> 2);
                uint ScreenY1 = (uint)(RDP.ScissorYH >> 2);
                uint ScreenX2 = (uint)(RDP.ScissorXL >> 2);
                uint ScreenY2 = (uint)(RDP.ScissorYL >> 2);

                if (RDP.ColorImageSize == (uint)RDP.BPP.BPP32)
                {
                    for (uint y = ScreenY1; y < ScreenY2; ++y)
                    {
                        uint xMin = Scanbuffer[(y * 2)    ];
                        uint xMax = Scanbuffer[(y * 2) + 1];

                        for (uint x = xMin; x < xMax; ++x)
                            SetPixel(x, y, RDP.FillColor, 4, ScreenX1, ScreenY1, ScreenX2, ScreenY2, ImageWidth);
                    }
                }
                else if (RDP.ColorImageSize == (uint)RDP.BPP.BPP16)
                {
                    for (uint y = ScreenY1; y < ScreenY2; ++y)
                    {
                        uint xMin = Scanbuffer[(y * 2)];
                        uint xMax = Scanbuffer[(y * 2) + 1];

                        for (uint x = xMin; x < xMax; x += 2)
                            SetPixel(x, y, RDP.FillColor, 2, ScreenX1, ScreenY1, ScreenX2, ScreenY2, ImageWidth);
                    }
                }
                else
                {
                    Common.Logger.PrintWarningLine($"RDP | {(RDP.BPP)RDP.ColorImageSize} isn't supported by RGBA");
                }
            }
            else
            {
                throw new NotImplementedException($"Fill drawing Scanbuffers aren't supported in any other mode other than RGBA currently.");
            }
        }

        private static void ScanConvertLine(uint[] Scanbuffer, float XStep, uint XStart, uint YStart, uint YEnd, uint Side)
        {
            float CurrX = XStart;

            for (uint y = YStart; y < YEnd; ++y)
            {
                Scanbuffer[(y * 2) + Side] = (uint)CurrX;
                CurrX += XStep;
            }
        }

        private static void ScanConvertTriangle(uint[] Scanbuffer, ushort YL, ushort YM, ushort YH, uint XL, uint DxLDy, uint XH, uint DxHDy, uint XM, uint DxMDy, byte Dir)
        {
            ushort DxLDyRound = (ushort)(DxLDy >> 16);
            ushort DxLDyFrac  = (ushort)(DxLDy & 0x0000FFFF);
            ushort DxHDyRound = (ushort)(DxHDy >> 16);
            ushort DxHDyFrac  = (ushort)(DxHDy & 0x0000FFFF);
            ushort DxMDyRound = (ushort)(DxMDy >> 16);
            ushort DxMDyFrac  = (ushort)(DxMDy & 0x0000FFFF);
            ushort XLRound    = (ushort)(XL    >> 16);
            ushort XHRound    = (ushort)(XH    >> 16);
            ushort XMRound    = (ushort)(XM    >> 16);

            ushort YLRound = (ushort)(YL >> 2);
            ushort YMRound = (ushort)(YM >> 2);
            ushort YHRound = (ushort)(YH >> 2);

            float XStep1 = (short)DxHDyRound + ((float)DxHDyFrac / 65535);
            float XStep2 = (short)DxLDyRound + ((float)DxLDyFrac / 65535);
            float XStep3 = (short)DxMDyRound + ((float)DxMDyFrac / 65535);

            ScanConvertLine(Scanbuffer, XStep1, XHRound, YHRound, YLRound, (uint)1 - Dir);
            ScanConvertLine(Scanbuffer, XStep2, XLRound, YMRound, YLRound, Dir);
            ScanConvertLine(Scanbuffer, XStep3, XMRound, YHRound, YMRound, Dir);
        }

        public static void FillTriangle(ushort YL, ushort YM, ushort YH, uint XL, uint DxLDy, uint XH, uint DxHDy, uint XM, uint DxMDy, byte Dir)
        {
            uint[] TriScanbuffer = new uint[480 * 2];

            ScanConvertTriangle(TriScanbuffer, YL, YM, YH, XL, DxLDy, XH, DxHDy, XM, DxMDy, Dir);

            DrawFillScanBuffer(TriScanbuffer);
        }

        public static void FillRect(ushort XL, ushort YL, ushort XH, ushort YH)
        {
            uint ImageWidth = RDP.ColorImageWidth + (uint)1;

            // Get the X and Y for the points of the rectangle by shifting the 10.2 format X and Y parameters by 2 to the right.
            uint RectX1 = (uint)(XH >> 2);
            uint RectY1 = (uint)(YH >> 2);
            uint RectX2 = (uint)(XL >> 2) + 1;
            uint RectY2 = (uint)(YL >> 2) + 1;

            // To find out where the Scanbuffer should stop.
            uint ScreenY2 = (uint)(RDP.ScissorYL >> 2);

            uint[] RectScanbuffer = new uint[480 * 2];

            for (uint y = RectY1; y < RectY2; ++y)
            {
                RectScanbuffer[(y * 2)    ] = RectX1;
                RectScanbuffer[(y * 2) + 1] = RectX2;
            }

            DrawFillScanBuffer(RectScanbuffer);
        }
    }
}
