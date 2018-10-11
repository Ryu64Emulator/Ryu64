using System;
using System.ComponentModel;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Ryu64.Graphics
{
    public class MainWindow : GameWindow
    {
        private const string BaseTitle = "Ryu64";

        public MainWindow() : base(640, 480, GraphicsMode.Default, BaseTitle, 
            GameWindowFlags.FixedWindow, 
            DisplayDevice.Default, 
            3, 3, 
            GraphicsContextFlags.ForwardCompatible)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
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

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Common.Util.Cleanup(MIPS.Registers.R4300.PC);

            MIPS.R4300.R4300_ON = false;
            MIPS.COP0.COP0_ON   = false;
            MIPS.COP1.COP1_ON   = false;

            Console.ResetColor();

            Environment.Exit(0);
        }
    }
}
