using OpenTK.Graphics.OpenGL;
using System;

namespace Ryu64.Graphics
{
    class LLERenderer : IRenderer
    {
        private static int FramebufferTexture;

        public void Init()
        {
            GL.Disable(EnableCap.DepthTest);

            FramebufferTexture = GL.GenTexture();
            GL.BindTexture (TextureTarget.Texture2D, FramebufferTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
        }

        public void Render()
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            RenderFramebuffer();
        }

        public void Update()
        {
        }

        public void Cleanup()
        {
            GL.DeleteTexture(FramebufferTexture);
        }

        private bool firstLoop = true;

        private unsafe void RenderFramebuffer()
        {
            uint VIStatus = MIPS.Cores.R4300.memory.ReadUInt32(0x04400000); // VI_STATUS_REG
            uint PixelSize = VIStatus & 0b000000000000000000000011;
            if (PixelSize == 0U) return;

            uint FramebufferWidth = MIPS.Cores.R4300.memory.ReadUInt32(0x04400008); // VI_H_WIDTH_REG

            byte Interlace = (byte)((VIStatus & 0b000000000000000001000000) >> 6);
            uint VIvStart = MIPS.Cores.R4300.memory.ReadUInt32(0x04400028);
            uint VerticalEndofVideo = VIvStart & 0x000003FF;
            uint VerticalStartOfVideo = (VIvStart >> 16) & 0x000003FF;

            uint FramebufferHeight = ((VerticalEndofVideo - VerticalStartOfVideo) + 6) >> (~Interlace & 0x01);

            uint FramebufferOrigin = MIPS.Cores.R4300.memory.ReadUInt32(0x04400004);

            byte[] Framebuffer = MIPS.Cores.R4300.memory.FastMemoryRead(FramebufferOrigin, (int)(FramebufferWidth * FramebufferHeight) *
                (PixelSize == 3U ? 4 : 2));

            if (firstLoop && Common.Variables.Debug)
                Common.Logger.PrintInfoLine($"Framebuffer: PixelSize: {PixelSize}, Width: {FramebufferWidth}, " +
                    $"Height: {FramebufferHeight}, Total Framebuffer Size: {Framebuffer.LongLength}, " +
                    $"Origin: 0x{FramebufferOrigin:X8}, Interlace: {Interlace}, " +
                    $"VerticalEndofVideo: {VerticalEndofVideo}, VerticalStartOfVideo: {VerticalStartOfVideo}");

            fixed (byte* p = Framebuffer)
            {
                IntPtr pFramebuffer = (IntPtr)p;

                if (PixelSize == 2U)
                {
                    int limit = Framebuffer.Length - (Framebuffer.Length % 2);
                    if (limit < 1) throw new Exception("Framebuffer too small to be swapped to Little Endian.");

                    for (int i = 0; i < limit - 1; i += 2)
                    {
                        byte temp = Framebuffer[i];
                        Framebuffer[i] = Framebuffer[i + 1];
                        Framebuffer[i + 1] = temp;
                    }

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                        (int)FramebufferWidth, (int)FramebufferHeight, 0, PixelFormat.Rgba, PixelType.UnsignedShort5551, pFramebuffer);
                }
                else if (PixelSize == 3U)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                        (int)FramebufferWidth, (int)FramebufferHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pFramebuffer);
                }

                GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
                GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
                GL.TexCoord2(1, 1); GL.Vertex2(1, -1);
                GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);

                GL.End();
            }
            firstLoop = false;
        }
    }
}
