using ImGuiNET;
using Ryu64.Formats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using IniParser;
using IniParser.Model;
using IniParser.Parser;

namespace Ryu64.GUI
{
    public class Window
    {
        private static Sdl2Window Win;
        private static GraphicsDevice GD;
        private static CommandList CL;
        private static ImGuiRenderer Renderer;

        [STAThread]
        public static void RunGUI()
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "Ryu64"),
                new GraphicsDeviceOptions(false, null, true),
                out Win,
                out GD);
            Win.Resized += () =>
            {
                GD.MainSwapchain.Resize((uint)Win.Width, (uint)Win.Height);
                Renderer.WindowResized(Win.Width, Win.Height);
            };

            CL = GD.ResourceFactory.CreateCommandList();
            Renderer = new ImGuiRenderer(GD, GD.MainSwapchain.Framebuffer.OutputDescription, Win.Width, Win.Height);

            InitUI();

            while (Win.Exists)
            {
                InputSnapshot snapshot = Win.PumpEvents();
                if (!Win.Exists) break;
                Renderer.Update(1f / 60f, snapshot);

                SubmitUI();

                CL.Begin();
                CL.SetFramebuffer(GD.MainSwapchain.Framebuffer);
                CL.ClearColorTarget(0, new RgbaFloat(0.5f, 0.5f, 0.5f, 1f));
                Renderer.Render(GD, CL);
                CL.End();
                GD.SubmitCommands(CL);
                GD.SwapBuffers(GD.MainSwapchain);
            }

            GD.WaitForIdle();
            Renderer.Dispose();
            CL.Dispose();
            GD.Dispose();
        }

        private static void InitEmulator(string RomPath)
        {
            Z64 Rom = new Z64(RomPath);
            Rom.Parse();

            if (!Directory.Exists(Common.Variables.AppdataFolder))
            {
                Directory.CreateDirectory(Common.Variables.AppdataFolder);
                Directory.CreateDirectory($"{Common.Variables.AppdataFolder}/saves");
            }

            Common.Settings.Parse($"{AppDomain.CurrentDomain.BaseDirectory}/Settings.ini");

            MIPS.R4300.memory = new MIPS.Memory(Rom.AllData);

            MIPS.R4300.PowerOnR4300(MIPS.R4300.TVType_enum.NTSC);
            if (Common.Settings.GRAPHICS_LLE)
                RDP.RDP.PowerOnRDP();

            Thread Graphics = new Thread(() =>
            {
                using (Graphics.MainWindow Window = new Graphics.MainWindow(Rom.Name.Trim())) Window.Run(60.0);
            })
            {
                Name = "GFX"
            };
            Graphics.Start();
        }

        private static bool[] WindowOpenState;
        private static string FileDialogRom_CurrPath;

        private static string FileDialogRom_FilePath = "";

        private static uint   MemoryViewer_CurrAddr = 0;
        private static byte[] MemoryViewer_AddrBuf  = new byte[1024];

        private static string DefaultPath = AppDomain.CurrentDomain.BaseDirectory;

        private static FileIniDataParser IniParser;
        private static IniData GUISettings;

        private static string GUISettingsPath = $"{AppDomain.CurrentDomain.BaseDirectory}GUISettings.ini";

        private static void LoadIni()
        {
            Common.Variables.Debug = bool.Parse(GUISettings.Global["debug"]);
        }

        private static void SaveIni()
        {
            GUISettings.Global["debug"] = Common.Variables.Debug.ToString();

            IniParser.WriteFile(GUISettingsPath, GUISettings);
        }

        private static unsafe void InitUI()
        {
            WindowOpenState = new bool[64];
            // Game list WIP
            // WindowOpenState[0] = true;
            FileDialogRom_CurrPath = DefaultPath;
            ImGui.GetStyle().WindowTitleAlign = new Vector2(0.5f, 0.5f);
            IniParser = new FileIniDataParser();
            GUISettings = IniParser.ReadFile(GUISettingsPath);

            LoadIni();
        }

        private static unsafe void MemoryViewer(ref uint Addr, ref bool WindowOpen, ref byte[] AddrBuf, string WindowTitle)
        {
            if (WindowOpen)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(570, 430), new Vector2(2048, 2048));
                if (ImGui.Begin(WindowTitle, ref WindowOpen, ImGuiWindowFlags.NoCollapse))
                {
                    byte[] Buf = new byte[256];

                    for (uint i = 0; i < Buf.Length; ++i)
                        Buf[i] = MIPS.R4300.memory[i + Addr, false];

                    if (AddrBuf[0] == 0)
                    {
                        string AddrStr = "00000000";

                        byte[] AddrStrBytes = Encoding.Default.GetBytes(AddrStr);

                        Buffer.BlockCopy(AddrStrBytes, 0, AddrBuf, 0, AddrStrBytes.Length);
                    }

                    ImGui.Text("Address: 0x");
                    ImGui.SameLine();
                    ImGui.InputText("##Address", AddrBuf, (uint)AddrBuf.Length, ImGuiInputTextFlags.CharsHexadecimal);

                    ImGui.SameLine();
                    if (ImGui.Button("Go"))
                        Addr = uint.Parse(Encoding.Default.GetString(AddrBuf).Split('\0')[0], NumberStyles.HexNumber);
                    if (ImGui.Button($"Sub {Buf.Length}"))
                    {
                        string AddrStr = Encoding.Default.GetString(AddrBuf);

                        uint AddrInt = uint.Parse(AddrStr, NumberStyles.HexNumber);

                        if (AddrInt >= Buf.Length)
                        {
                            AddrStr = (AddrInt - Buf.Length).ToString("X8");

                            byte[] AddrStrBytes = Encoding.Default.GetBytes(AddrStr);

                            Buffer.BlockCopy(AddrStrBytes, 0, AddrBuf, 0, AddrStrBytes.Length);
                        }
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($"Add {Buf.Length}"))
                    {
                        string AddrStr = Encoding.Default.GetString(AddrBuf);

                        uint AddrInt = uint.Parse(AddrStr, NumberStyles.HexNumber);

                        if (AddrInt <= uint.MaxValue)
                        {
                            AddrStr = (AddrInt + Buf.Length).ToString("X8");

                            byte[] AddrStrBytes = Encoding.Default.GetBytes(AddrStr);

                            Buffer.BlockCopy(AddrStrBytes, 0, AddrBuf, 0, AddrStrBytes.Length);
                        }
                    }

                    ImGui.Separator();
                    for (uint i = 0; i < Buf.Length; ++i)
                    {
                        if (i % 16 != 0) ImGui.SameLine();
                        ImGui.Text($"0x{Buf[i]:X2}");
                    }

                    ImGui.End();
                }
            }
        }

        private static unsafe bool FileBrowser(ref string Path, ref string FilePath, ref bool WindowOpen, string WindowTitle)
        {
            bool Result = false;

            if (WindowOpen)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(798, 456), new Vector2(2048, 2048));
                if (ImGui.Begin(WindowTitle, ref WindowOpen, ImGuiWindowFlags.NoCollapse))
                {
                    string[] DecompPath = Path.Split('\\', '/');

                    for (uint i = 0; i < DecompPath.Length; ++i)
                    {
                        string Str = DecompPath[i];
                        if (string.IsNullOrWhiteSpace(Str)) break;
                        ImGui.SameLine();
                        if (ImGui.Button(Str))
                        {
                            string NewPath = "";
                            for (uint j = 0; j <= i; ++j)
                                NewPath += DecompPath[j] + '\\';
                            Path = NewPath;
                        }
                    }
                    ImGui.Separator();

                    IEnumerable<string> Dirs  = Directory.EnumerateDirectories(Path);
                    IEnumerable<string> Files = Directory.EnumerateFiles(Path);

                    if (ImGui.BeginChild("##FileDialog_Drives", new Vector2(256, 256)))
                    {
                        foreach (string Drive in Directory.GetLogicalDrives())
                        {
                            if (ImGui.Selectable(Drive))
                                Path = Drive;
                        }
                        ImGui.EndChild();
                    }
                    ImGui.SameLine();

                    Vector2 size = ImGui.GetContentRegionMax() - new Vector2(0.0f, 120.0f);

                    if (ImGui.BeginChild("##FileDialog_FileList", size))
                    {
                        foreach (string Dir in Dirs)
                            if (ImGui.Selectable($"[Dir] {System.IO.Path.GetFileName(Dir)}"))
                                Path = Dir;

                        foreach (string File in Files)
                            if (ImGui.Selectable($"[File] {System.IO.Path.GetFileName(File)}"))
                                FilePath = File;
                        ImGui.EndChild();
                    }

                    byte[] FilenameBuffer = new byte[1024];

                    byte[] FilePathBytes = Encoding.Default.GetBytes(FilePath);

                    Buffer.BlockCopy(FilePathBytes, 0, FilenameBuffer, 0, FilePathBytes.Length);

                    float Width = ImGui.GetContentRegionAvailWidth();
                    ImGui.PushItemWidth(Width);
                    ImGui.InputText("FileName", FilenameBuffer, (uint)FilenameBuffer.Length);
                    ImGui.PopItemWidth();

                    if (ImGui.Button("Ok"))
                    {
                        WindowOpen = string.IsNullOrEmpty(FilePath);
                        Result = !string.IsNullOrEmpty(FilePath);
                    }

                    ImGui.SameLine();

                    if (ImGui.Button("Cancel"))
                    {
                        WindowOpen = false;
                        Result = false;
                    }

                    FilePath = Encoding.Default.GetString(FilenameBuffer).Split('\0')[0];

                    ImGui.End();
                }
            }
            return Result;
        }

        private static unsafe void SubmitMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open ROM", !MIPS.R4300.R4300_ON))
                        WindowOpenState[2] = true;
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Settings"))
                    {
                        WindowOpenState[1] = true;
                        LoadIni();
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.BeginMenu("Debugger", MIPS.R4300.R4300_ON))
                    {
                        if (ImGui.MenuItem("Register Viewer"))
                            WindowOpenState[3] = true;

                        if (ImGui.MenuItem("Memory Viewer"))
                            WindowOpenState[5] = true;
                        ImGui.EndMenu();
                    }

                    // For Debugging the GUI only.
                    /*
                    if (ImGui.BeginMenu("ImGui"))
                    {
                        if (ImGui.MenuItem("Style Editor"))
                            WindowOpenState[4] = true;
                        ImGui.EndMenu();
                    }
                    */
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        private static unsafe void SubmitUI()
        {
            ImGui.StyleColorsDark();

            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize,     15);
            ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize,       8);

            ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding,      4);
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 2);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,     4);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,    7);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowTitleAlign,  0.5f);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize,  0);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0);

            ImGui.PushStyleColor(ImGuiCol.WindowBg,      0xFF0F0F0F);
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, 0xFF202020);

            ImGui.PushStyleColor(ImGuiCol.ButtonActive,  0xFF707070);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFF404040);
            ImGui.PushStyleColor(ImGuiCol.Button,        0xFF202020);

            ImGui.PushStyleColor(ImGuiCol.FrameBgActive,  0xFF707070);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, 0xFF404040);
            ImGui.PushStyleColor(ImGuiCol.FrameBg,        0xFF202020);

            ImGui.PushStyleColor(ImGuiCol.TabActive,  0xFF707070);
            ImGui.PushStyleColor(ImGuiCol.TabHovered, 0xFF404040);
            ImGui.PushStyleColor(ImGuiCol.Tab,        0xFF202020);

            ImGui.PushStyleColor(ImGuiCol.TabUnfocused,       0xFF202020);
            ImGui.PushStyleColor(ImGuiCol.TabUnfocusedActive, 0xFF707070);

            ImGui.PushStyleColor(ImGuiCol.HeaderActive,  0xFF707070);
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, 0xFF404040);
            ImGui.PushStyleColor(ImGuiCol.Header,        0xFF202020);

            ImGui.PushStyleColor(ImGuiCol.CheckMark, 0xFF7E7E7E);
            ImGui.PushStyleColor(ImGuiCol.SliderGrab, 0xFF7E7E7E);
            ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, 0xFF808080);

            ImGui.PushStyleColor(ImGuiCol.NavHighlight, 0xFF404040);

            SubmitMenuBar();

            if (WindowOpenState[0])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(256, 256), new Vector2(2048, 2048));
                if (ImGui.Begin("Games", ref WindowOpenState[0], ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Text("Game List");
                    ImGui.Separator();
                    ImGui.End();
                }
            }

            if (WindowOpenState[1])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(735, 464), new Vector2(2048, 2048));
                if (ImGui.Begin("Settings", ref WindowOpenState[1], ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Checkbox("Debug", ref Common.Variables.Debug);
                    if (ImGui.Button("Apply"))
                        SaveIni();
                    ImGui.End();
                }
            }

            if (WindowOpenState[3])
            {
                if (!MIPS.R4300.R4300_ON) WindowOpenState[3] = false;
                ImGui.SetNextWindowSizeConstraints(new Vector2(256, 550), new Vector2(2048, 2048));
                if (ImGui.Begin("Register Viewer", ref WindowOpenState[3]))
                {
                    ImGui.Text($"R4300:");
                    for (uint i = 0; i < MIPS.Registers.R4300.Reg.Length; ++i)
                        ImGui.Text($" R[{i}]: 0x{MIPS.Registers.R4300.Reg[i]:X16}");
                    ImGui.Separator();
                    ImGui.Text($"COP0:");
                    for (uint i = 0; i < MIPS.Registers.COP0.Reg.Length; ++i)
                        ImGui.Text($" COP0R[{i}]: 0x{MIPS.Registers.COP0.Reg[i]:X16}");
                    ImGui.Separator();
                    ImGui.Text($"RSP:");
                    for (uint i = 0; i < MIPS.Registers.RSPReg.Reg.Length; ++i)
                        ImGui.Text($" RSPR[{i}]: 0x{MIPS.Registers.RSPReg.Reg[i]:X8}");
                    ImGui.Separator();
                    ImGui.Text($"RSPCOP0:");
                    for (uint i = 0; i < MIPS.Registers.RSPCOP0.Reg.Length; ++i)
                        ImGui.Text($" RSPC0R[{i}]: 0x{MIPS.Registers.RSPCOP0.Reg[i]:X16}");

                    ImGui.End();
                }
            }

            if (WindowOpenState[4])
            {
                if (ImGui.Begin("Style Editor"))
                {
                    ImGui.ShowStyleEditor();
                    ImGui.End();
                }
            }

            if (WindowOpenState[5] && !MIPS.R4300.R4300_ON) WindowOpenState[5] = false;
            MemoryViewer(ref MemoryViewer_CurrAddr, ref WindowOpenState[5], ref MemoryViewer_AddrBuf, "Memory Viewer");

            if (FileBrowser(ref FileDialogRom_CurrPath, ref FileDialogRom_FilePath, ref WindowOpenState[2], "Select a ROM to load."))
            {
                InitEmulator(FileDialogRom_FilePath);

                FileDialogRom_FilePath = "";
            }

            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
        }
    }
}
