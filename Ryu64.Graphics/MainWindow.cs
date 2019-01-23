using System;
using System.ComponentModel;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Ryu64.RDP;

namespace Ryu64.Graphics
{
    public class MainWindow : GameWindow
    {
        private static string BaseTitle = "Ryu64";
        private static IRenderer Renderer;

        public MainWindow(string GameName) : base(960, 720, GraphicsMode.Default, BaseTitle, 
            GameWindowFlags.Default, 
            DisplayDevice.Default, 
            3, 1,
            GraphicsContextFlags.ForwardCompatible)
        {
            BaseTitle += $" | {GameName}";
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport((Width / 2) - (960 / 2), (Height / 2) - (720 / 2), 960, 720);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Common.Settings.GRAPHICS_LLE)
                Renderer = new LLERenderer();
            else
                Renderer = new HLERenderer();

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Renderer.Init();
        }

        bool LastFrameF11;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Renderer.Update();

            Title = $"{BaseTitle} | FPS: {RenderFrequency:N2} | CPU MHz: {Common.Variables.CPUMHz:N2}";

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Key.Escape)) Exit();
            if (LastFrameF11 && KeyboardState.IsKeyUp(Key.F11))
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;

            LastFrameF11 = KeyboardState.IsKeyDown(Key.F11);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Renderer.Render();

            SwapBuffers();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Renderer.Cleanup();

            Common.Util.Cleanup(MIPS.Registers.R4300.PC);

            MIPS.R4300.R4300_ON = false;
            MIPS.COP0.COP0_ON   = false;
            MIPS.COP1.COP1_ON   = false;

            Console.ResetColor();

            Environment.Exit(0);
        }
    }
}
