using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImGuiOpenTK
{
    public class OpenTKWindow : GameWindow
    {

        protected IntPtr _GLContext;
        public IntPtr GLContext => _GLContext;

        //public GameWindowFlags Flags =>;

        public Action<OpenTKWindow> OnLoop;
        public bool IsAlive = false;

        public OpenTKWindow(
            string title = "OpenTK Window",
            int width = 800, int height = 600
            ) : base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default)
        {
        }

        public bool IsVisible => Visible;
        public void Show() => Visible = true;
        public void Hide() => Visible = false;

        public virtual void Start()
        {
            Show();
            Run(60, 60);
        }

        protected override void OnLoad(EventArgs e)
        {
            IsAlive = true;
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IsAlive = false;
            base.OnClosing(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            OnLoop.Invoke(this);
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        public virtual void Swap() => SwapBuffers();

        protected override void Dispose(bool manual)
        {
            base.Dispose(manual);
        }

        ~OpenTKWindow()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}