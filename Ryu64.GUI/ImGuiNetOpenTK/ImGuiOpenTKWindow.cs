using ImGuiNET;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.IO;
using OpenTK.Graphics;

/*
Based off of this implementation and heavily modified to work with new versions of ImGUI.NET and OpenTK:
https://github.com/emmauss/ImGuiNet.OpentK
*/

namespace ImGuiOpenTK
{
    public class ImGuiOpenTKWindow : OpenTKWindow
    {
        protected readonly bool[] g_MousePressed = { false, false, false };
        protected int g_FontTexture = 0;
        protected float g_MouseWheel = 0.0f;
        protected IntPtr GuiContext;
        protected MouseState _mouse;

        public System.Numerics.Vector2 Position
        {
            get
            {
                return new System.Numerics.Vector2(X, Y);
            }
            set
            {
                X = (int)Math.Round(value.X);
                Y = (int)Math.Round(value.Y);
            }
        }

        public new Point Size
        {
            get
            {
                return new Point(Width, Height);
            }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public ImGuiOpenTKWindow(
            string title = "ImGui.NET-OpenTK-CS Window",
            int width = 800, int height = 600) : base(title, width, height)
        {
            GuiContext = ImGui.CreateContext();
            ImGui.SetCurrentContext(GuiContext);

            ImGui.GetIO().Fonts.AddFontDefault();

            ImGuiOpenTKHelper.Init();
            OnLoop = ImGuiOnLoop;
        }

        public void ImGuiOnLoop(OpenTKWindow Window)
        {
            GL.ClearColor(0.1f, 0.125f, 0.15f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            ImGuiRender();
        }

        public override void Start()
        {
            if (!File.Exists("imgui.ini"))
                File.WriteAllText("imgui.ini", "");

            Create();

            base.Start();
        }

        private unsafe void UpdateImGuiInput(ImGuiIOPtr io)
        {
            if (_mouse != null)
            {
                if (Focused)
                {
                    io.MousePos = new System.Numerics.Vector2(_mouse.X, _mouse.Y);
                }
                else
                {
                    io.MousePos = new System.Numerics.Vector2(-1f, -1f);
                }

                io.MouseDown[0] = _mouse.LeftButton == ButtonState.Pressed;
                io.MouseDown[1] = _mouse.RightButton == ButtonState.Pressed;
                io.MouseDown[2] = _mouse.MiddleButton == ButtonState.Pressed;

                float newWheelPos = _mouse.WheelPrecise;
                float delta = newWheelPos - g_MouseWheel;
                g_MouseWheel = newWheelPos;
                io.MouseWheel += delta;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ImGuiIOPtr IO = ImGui.GetIO();

            IO.AddInputCharactersUTF8($"{e.KeyChar}");
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            ImGuiIOPtr IO = ImGui.GetIO();

            IO.KeysDown[(int)e.Key] = true;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            ImGuiIOPtr IO = ImGui.GetIO();

            IO.KeysDown[(int)e.Key] = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _mouse = e.Mouse;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _mouse = e.Mouse;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            _mouse = e.Mouse;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _mouse = e.Mouse;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.ColorBufferBit);
            base.OnUpdateFrame(e);
            UpdateImGuiInput(ImGui.GetIO());
            ImGuiRender();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Swap();
        }

        public virtual void ImGuiRender()
        {
            //if(Mouse?.GetState().LeftButton == OpenTK.Input.ButtonState.Pressed)
            ImGuiOpenTKHelper.NewFrame(Size, System.Numerics.Vector2.One);

            ImGuiLayout();

            ImGuiOpenTKHelper.Render(Size);
        }

        public virtual void ImGuiLayout()
        {
        }

        protected unsafe virtual void Create()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            // Build texture atlas
            io.Fonts.GetTexDataAsAlpha8(out byte* TexPixels, out int TexWidth, out int TexHeight);

            // Create OpenGL texture
            g_FontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, g_FontTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                TexWidth,
                TexHeight,
                0,
                PixelFormat.Alpha,
                PixelType.UnsignedByte,
                new IntPtr(TexPixels));

            // Store the texture identifier in the ImFontAtlas substructure.
            io.Fonts.SetTexID((IntPtr)g_FontTexture);

            // Cleanup (don't clear the input data if you want to append new fonts later)
            io.Fonts.ClearTexData();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void Dispose(bool disposing)
        {
            ImGui.DestroyContext(GuiContext);
            /*ImGuiIO io = ImGui.GetIO();

            if (disposing) {
                // Dispose managed state (managed objects).
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            if (g_FontTexture != 0) {
                // Texture gets deleted with the context.
                // GL.DeleteTexture(g_FontTexture);
                if ((int) io.FontAtlas.TexID == g_FontTexture)
                    io.FontAtlas.TexID = IntPtr.Zero;
                g_FontTexture = 0;
            }
            */
            base.Dispose(disposing);
        }

        ~ImGuiOpenTKWindow()
        {
            Dispose(false);
        }
    }
}
