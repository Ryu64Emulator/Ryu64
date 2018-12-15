using System;
using System.ComponentModel;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Ryu64.Graphics
{
    public class MainWindow : GameWindow
    {
        private static string BaseTitle = "Ryu64";
        private static uint  CurrentScanline = 0;
        private static int   FramebufferTexture;

        public MainWindow(string GameName) : base(960, 720, GraphicsMode.Default, BaseTitle, 
            GameWindowFlags.FixedWindow, 
            DisplayDevice.Default, 
            3, 1,
            GraphicsContextFlags.ForwardCompatible)
        {
            BaseTitle += $" | {GameName}";

            Thread ScanlineThread = new Thread(() =>
            {
                while (MIPS.R4300.R4300_ON)
                {
                    ++CurrentScanline;
                    MIPS.R4300.memory.WriteUInt32(0x04400010, CurrentScanline); // VI_CURRENT_REG
                    if (CurrentScanline == 525)
                        CurrentScanline = 0;
                }
            });
            ScanlineThread.Name = "VIScanlineThread";
            ScanlineThread.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (Common.Settings.GRAPHICS_LLE)
            {
                GL.Disable(EnableCap.DepthTest);

                FramebufferTexture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, FramebufferTexture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Title = $"{BaseTitle} | FPS: {(int)RenderFrequency}";

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Key.Escape)) Exit();
            if (Common.Settings.STEP_MODE && KeyboardState.IsKeyDown(Key.Enter)) Common.Variables.Step = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(0, 0, 0, 1);
            if (Common.Settings.GRAPHICS_LLE)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                RenderFramebufferDirect();
            }
            else
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            }

            SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (Common.Settings.GRAPHICS_LLE)
                GL.DeleteTexture(FramebufferTexture);

            Common.Util.Cleanup(MIPS.Registers.R4300.PC);

            MIPS.R4300.R4300_ON = false;
            MIPS.COP0.COP0_ON   = false;
            MIPS.COP1.COP1_ON   = false;

            Console.ResetColor();

            Environment.Exit(0);
        }

        private unsafe void RenderFramebufferDirect()
        {
            uint VIStatus = MIPS.R4300.memory.ReadUInt32(0x04400000); // VI_STATUS_REG
            uint PixelSize = VIStatus & 0b000000000000000000000011;
            if (PixelSize == 0U) return;

            uint FramebufferWidth = MIPS.R4300.memory.ReadUInt32(0x04400008); // VI_H_WIDTH_REG

            byte Interlace = (byte)((VIStatus & 0b000000000000000001000000) >> 6);
            uint VIvStart = MIPS.R4300.memory.ReadUInt32(0x04400028);
            uint VerticalEndofVideo   =  VIvStart        & 0x000003FF;
            uint VerticalStartOfVideo = (VIvStart >> 16) & 0x000003FF;

            uint FramebufferHeight = ((VerticalEndofVideo - VerticalStartOfVideo) + 6) >> (~Interlace & 0x01);

            uint FramebufferOrigin = MIPS.R4300.memory.ReadUInt32(0x04400004);

            byte[] Framebuffer = MIPS.R4300.memory.FastMemoryRead(FramebufferOrigin, (int)(FramebufferWidth * FramebufferHeight) * 
                (PixelSize == 3U ? 4 : 2));

            IntPtr pFramebuffer;

            fixed (byte* p = Framebuffer)
                pFramebuffer = (IntPtr)p;

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

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    (int)FramebufferWidth, (int)FramebufferHeight, 0, PixelFormat.Rgba, PixelType.UnsignedShort5551, pFramebuffer);
            }
            else if (PixelSize == 3U)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    (int)FramebufferWidth, (int)FramebufferHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pFramebuffer);
            }

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 0); GL.Vertex2(-1, 1);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 1);
            GL.TexCoord2(1, 1); GL.Vertex2(1, -1);
            GL.TexCoord2(0, 1); GL.Vertex2(-1, -1);

            GL.End();
        }
    }
}
