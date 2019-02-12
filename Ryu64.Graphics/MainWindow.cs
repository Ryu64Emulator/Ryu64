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
        private static string GameName;
        private static IRenderer Renderer;

        public MainWindow(string GameName) : base(960, 720, GraphicsMode.Default, BaseTitle, 
            GameWindowFlags.FixedWindow, 
            DisplayDevice.Default, 
            3, 1,
            GraphicsContextFlags.ForwardCompatible)
        {
            MainWindow.GameName = GameName;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
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
        bool LastFrameF1;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Renderer.Update();

            string Paused = Common.Variables.Pause ? " | Paused" : "";
            Title = $"{BaseTitle} | {GameName} | FPS: {RenderFrequency:N2} | CPU MHz: {Common.Variables.CPUMHz:N2}{Paused}";

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Key.Escape)) Exit();
            if (LastFrameF11 && KeyboardState.IsKeyUp(Key.F11))
                if (WindowState != WindowState.Fullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                    WindowState = WindowState.Normal;
            if (LastFrameF1 && KeyboardState.IsKeyUp(Key.F1) && Common.Variables.Pause)
                Common.Variables.Step = true;
            

            LastFrameF11 = KeyboardState.IsKeyDown(Key.F11);
            LastFrameF1  = KeyboardState.IsKeyDown(Key.F1);
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

            MIPS.Cores.R4300.R4300_ON = false;
            MIPS.Cores.RSP.RSP_ON     = false;
            if (Common.Settings.GRAPHICS_LLE)
                MIPS.RDPWrapper.RDPThread.Interrupt();

            Console.ResetColor();
        }
    }
}
