using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using ImGuiNET;
using TKEventType = ImGuiOpenTK.TKEvent.Type;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ImGuiOpenTK
{
    public static class ImGuiOpenTKHelper
    {
        public static bool Initialized { get; private set; } = false;

        public static void Init()
        {
            if (Initialized)
                return;
            Initialized = true;

            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;

        }

        public unsafe static void NewFrame(Point size, System.Numerics.Vector2 scale)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(size.X, size.Y);
            io.DisplayFramebufferScale = scale;
            io.DeltaTime = 1f / 60f;

            ImGui.NewFrame();
        }

        public unsafe static void Render(Point size)
        {
            ImGui.Render();
            if (ImGui.GetIO().RenderDrawListsFnUnused == IntPtr.Zero)
                RenderDrawData(ImGui.GetDrawData(), size.X, size.Y);
        }

        public unsafe static void RenderDrawData(ImDrawDataPtr drawData, int displayW, int displayH)
        {
            // We are using the OpenGL fixed pipeline to make the example code simpler to read!
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers.
            System.Numerics.Vector4 clear_color = new System.Numerics.Vector4(114f / 255f, 144f / 255f, 154f / 255f, 1.0f);
            GL.Viewport(0, 0, displayW, displayH);
            GL.ClearColor(clear_color.X, clear_color.Y, clear_color.Z, clear_color.W);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            int last_texture;
            GL.GetInteger(GetPName.TextureBinding2D, out last_texture);
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            ImGuiIOPtr io = ImGui.GetIO();
            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = drawData.CmdListsRange[n];
                byte* vtx_buffer = (byte*)cmd_list.VtxBuffer.Data;
                ushort* idx_buffer = (ushort*)cmd_list.IdxBuffer.Data;

                GL.VertexPointer(2, VertexPointerType.Float, Unsafe.SizeOf<ImDrawVert>(), new IntPtr(vtx_buffer));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Unsafe.SizeOf<ImDrawVert>(), new IntPtr(vtx_buffer + 8));
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, Unsafe.SizeOf<ImDrawVert>(), new IntPtr(vtx_buffer + 16));

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, pcmd.TextureId.ToInt32());
                        GL.Scissor(
                            (int)pcmd.ClipRect.X,
                            (int)(io.DisplaySize.Y - pcmd.ClipRect.W),
                            (int)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (int)(pcmd.ClipRect.W - pcmd.ClipRect.Y));
                        ushort[] indices = new ushort[pcmd.ElemCount];
                        for (int i = 0; i < indices.Length; i++) { indices[i] = idx_buffer[i]; }
                        GL.DrawElements(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, new IntPtr(idx_buffer));
                    }
                    idx_buffer += pcmd.ElemCount;
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindTexture(TextureTarget.Texture2D, last_texture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.PopAttrib();
        }



        public static bool HandleEvent(TKEvent tKEvent)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            switch (tKEvent.EventType)
            {
                case TKEventType.Keyboard:
                    var KeyboardEvent = tKEvent as KeyBoardEvent;
                    switch (KeyboardEvent.Key)
                    {
                        case Key.ControlLeft:
                        case Key.ControlRight:
                            io.KeyCtrl = KeyboardEvent.IsKeyDown;
                            break;
                        case Key.ShiftLeft:
                        case Key.ShiftRight:
                            io.KeyShift = KeyboardEvent.IsKeyDown;
                            break;
                        case Key.AltLeft:
                        case Key.AltRight:
                            io.KeyAlt = KeyboardEvent.IsKeyDown;
                            break;
                        default:
                            io.KeysDown[(int)KeyboardEvent.Key] = KeyboardEvent.IsKeyDown;
                            break;
                    }
                    break;
                case TKEventType.MouseButton:
                    var MouseButton = tKEvent as MouseButtonEvent;
                    switch (MouseButton.Button)
                    {
                        case OpenTK.Input.MouseButton.Left:
                            io.MouseDown[0] = MouseButton.IsButtonDown;
                            break;
                        case OpenTK.Input.MouseButton.Right:
                            io.MouseDown[1] = MouseButton.IsButtonDown;
                            break;
                        case OpenTK.Input.MouseButton.Middle:
                            io.MouseDown[2] = MouseButton.IsButtonDown;
                            break;
                    }
                    break;
                case TKEventType.MouseWheel:
                    var MouseWheel = tKEvent as MouseWheelEvent;
                    io.MouseWheel = MouseWheel.Value;
                    break;
                case TKEventType.MouseMotion:
                    var MouseMotion = tKEvent as MouseMotionEvent;
                    io.MousePos = new System.Numerics.Vector2(MouseMotion.Position.X, MouseMotion.Position.Y);
                    break;
                case TKEventType.TextInput:
                    var TextEvent = tKEvent as TextInputEvent;
                    unsafe
                    {
                        io.AddInputCharactersUTF8(TextEvent.Text);
                    }
                    break;

            }


            /*switch (mouse.GetState) {
                 case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                     if (e.wheel.y > 0)
                         mouseWheel = 1;
                     if (e.wheel.y < 0)
                         mouseWheel = -1;
                     return true;
                 case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                     if (mousePressed == null)
                         return true;
                     if (e.button.button == SDL.SDL_BUTTON_LEFT && mousePressed.Length > 0)
                         mousePressed[0] = true;
                     if (e.button.button == SDL.SDL_BUTTON_RIGHT && mousePressed.Length > 1)
                         mousePressed[1] = true;
                     if (e.button.button == SDL.SDL_BUTTON_MIDDLE && mousePressed.Length > 2)
                         mousePressed[2] = true;
                     return true;
                 case SDL.SDL_EventType.SDL_TEXTINPUT:
                     unsafe
                     {
                         // THIS IS THE ONLY UNSAFE THING LEFT!
                         ImGui.AddInputCharactersUTF8(e.text.text);
                     }
                     return true;
                 case SDL.SDL_EventType.SDL_KEYDOWN:
                 case SDL.SDL_EventType.SDL_KEYUP:
                     int key = (int) e.key.keysym.sym & ~SDL.SDLK_SCANCODE_MASK;
                     io.KeysDown[key] = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                     SDL.SDL_Keymod keyModState = SDL.SDL_GetModState();
                     io.ShiftPressed = (keyModState & SDL.SDL_Keymod.KMOD_SHIFT) != 0;
                     io.CtrlPressed = (keyModState & SDL.SDL_Keymod.KMOD_CTRL) != 0;
                     io.AltPressed = (keyModState & SDL.SDL_Keymod.KMOD_ALT) != 0;
                     io.SuperPressed = (keyModState & SDL.SDL_Keymod.KMOD_GUI) != 0;
                     return true;
             }
             */
            return true;
        }

    }
}