using ImGuiNET;
using Ryu64.Formats;
using System;
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

            if (!Rom.HasBeenParsed)
            {
                WindowOpenState[7] = true;
                return;
            }

            if (!Directory.Exists(Common.Variables.AppdataFolder))
            {
                Directory.CreateDirectory(Common.Variables.AppdataFolder);
                Directory.CreateDirectory($"{Common.Variables.AppdataFolder}saves");
            }

            Common.Settings.Parse($"{AppDomain.CurrentDomain.BaseDirectory}Settings.ini");

            MIPS.Cores.R4300.memory = new MIPS.Memory(Rom.AllData);

            MIPS.Cores.R4300.PowerOnR4300(MIPS.Cores.R4300.TVType_enum.NTSC);
            if (Common.Settings.GRAPHICS_LLE)
            {
                RDP.RDP.PowerOnRDP();
                MIPS.Cores.RSP.PowerOnRSP();
            }

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

        private static string FolderDialogRoms_CurrPath;
        private static string FolderDialogRoms_FilePath = "";

        private static bool   HideDotFilesOnUnix;
        private static string RomDirectory;

        private static bool   GraphicsLLE;
        private static bool   ExpansionPak;

        private static uint   MemoryViewer_CurrAddr = 0;
        private static byte[] MemoryViewer_AddrBuf  = new byte[1024];

        private static uint   Disasm_CurrAddr = 0;
        private static byte[] Disasm_AddrBuf = new byte[1024];

        private static string DefaultPath = AppDomain.CurrentDomain.BaseDirectory;

        private static IEnumerable<string> Roms;

        private static FileIniDataParser IniParser;
        private static IniData GUISettings;
        private static IniData EMUSettings;

        private static string GUISettingsPath = $"{AppDomain.CurrentDomain.BaseDirectory}GUISettings.ini";
        private static string EMUSettingsPath = $"{AppDomain.CurrentDomain.BaseDirectory}Settings.ini";

        private static void LoadIni()
        {
            Common.Variables.Debug = bool.Parse(GUISettings.Global["debug"]);
            HideDotFilesOnUnix     = bool.Parse(GUISettings.Global["hidedotfilesunix"]);
            RomDirectory           = GUISettings.Global["romdir"];
            GraphicsLLE            = bool.Parse(EMUSettings.Global["GRAPHICS_LLE"]);
            ExpansionPak           = bool.Parse(EMUSettings.Global["EXPANSION_PAK"]);
        }

        private static void SaveIni()
        {
            GUISettings.Global["debug"]            = Common.Variables.Debug.ToString();
            GUISettings.Global["hidedotfilesunix"] = HideDotFilesOnUnix.ToString();
            GUISettings.Global["romdir"]           = RomDirectory;

            EMUSettings.Global["GRAPHICS_LLE"]  = GraphicsLLE.ToString();
            EMUSettings.Global["EXPANSION_PAK"] = ExpansionPak.ToString();

            IniParser.WriteFile(GUISettingsPath, GUISettings);
            IniParser.WriteFile(EMUSettingsPath, EMUSettings);
        }

        private static unsafe void InitUI()
        {
            WindowOpenState = new bool[64];
            WindowOpenState[0] = true;

            FileDialogRom_CurrPath    = DefaultPath;
            FolderDialogRoms_CurrPath = DefaultPath;

            ImGui.GetStyle().WindowTitleAlign = new Vector2(0.5f, 0.5f);

            IniParser   = new FileIniDataParser();
            GUISettings = IniParser.ReadFile(GUISettingsPath);
            EMUSettings = IniParser.ReadFile(EMUSettingsPath);
            Roms        = new string[1];
            LoadIni();
            ReloadRoms();
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
                        Buf[i] = MIPS.Cores.R4300.memory[i + Addr, false];

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

        private static unsafe bool FileBrowser(ref string Path, ref string FilePath, ref bool WindowOpen, ref bool PermDenyOpen, ref bool HideDotFilesOnUnix, string WindowTitle, bool FileSelect)
        {
            bool Result = false;

            if (PermDenyOpen)
            {
                ImGui.PushStyleColor(ImGuiCol.WindowBg, 0xEE0F0F0F);
                ImGui.SetNextWindowSizeConstraints(new Vector2(256, 64), new Vector2(2048, 2048));
                if (ImGui.Begin("Permission Denied.", ref PermDenyOpen, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
                {
                    ImGui.Text("Permission to access this directory is denied.");
                    if (ImGui.Button("Ok"))
                        PermDenyOpen = false;
                    ImGui.End();
                }
                ImGui.PopStyleColor(1);
            }

            if (WindowOpen)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(798, 456), new Vector2(2048, 2048));
                if (ImGui.Begin(WindowTitle, ref WindowOpen, ImGuiWindowFlags.NoCollapse))
                {
                    string[] DecompPath = Path.Split('\\', '/');

                    for (uint i = 0; i < DecompPath.Length; ++i)
                    {
                        string Str = DecompPath[i];
                        if (string.IsNullOrWhiteSpace(Str)) continue;
                        ImGui.SameLine();
                        if (ImGui.Button(Str))
                        {
                            string NewPath = "";
                            for (uint j = 0; j <= i; ++j)
                                NewPath += DecompPath[j] + '/';
                            Path = NewPath;
                        }
                    }
                    ImGui.Separator();

                    if (ImGui.BeginChild("##FileDialog_Drives", new Vector2(256, ImGui.GetContentRegionMax().Y - 120.0f)))
                    {
                        if (Environment.OSVersion.Platform == PlatformID.Unix)
                        {
                            if (ImGui.Selectable("/home"))
                                Path = "/home";
                            if (ImGui.Selectable("/"))
                                Path = "/";
                        }
                        else
                        {
                            foreach (DriveInfo Drive in DriveInfo.GetDrives())
                            {
                                if (ImGui.Selectable((!Drive.IsReady) ? $"{Drive.Name} (Not Ready)" : Drive.Name, 
                                        false, (!Drive.IsReady) ? ImGuiSelectableFlags.Disabled : 0))
                                    Path = Drive.Name;
                            }
                        }
                        
                        ImGui.EndChild();
                    }
                    ImGui.SameLine();

                    Vector2 size = ImGui.GetContentRegionMax() - new Vector2(0.0f, 120.0f);
                    IEnumerable<string> Dirs  = Directory.EnumerateDirectories(Path);
                    IEnumerable<string> Files = Directory.EnumerateFiles(Path);

                    if (ImGui.BeginChild("##FileDialog_FileList", size))
                    {
                        foreach (string Dir in Dirs)
                        {
                            if (HideDotFilesOnUnix
                                && Environment.OSVersion.Platform == PlatformID.Unix
                                && System.IO.Path.GetFileName(Dir)[0] == '.')
                                continue;

                            if (ImGui.Selectable($"[Dir] {System.IO.Path.GetFileName(Dir)}"))
                            {
                                try
                                {
                                    Directory.EnumerateDirectories(Dir);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    PermDenyOpen = true;
                                    continue;
                                }
                                Path = Dir;
                            }
                        }

                        if (FileSelect)
                        {
                            foreach (string File in Files)
                            {
                                if (HideDotFilesOnUnix
                                    && Environment.OSVersion.Platform == PlatformID.Unix
                                    && System.IO.Path.GetFileName(File)[0] == '.')
                                    continue;

                                if (ImGui.Selectable($"[File] {System.IO.Path.GetFileName(File)}"))
                                    FilePath = File;
                            }
                        }
                        ImGui.EndChild();
                    }

                    if (!FileSelect) FilePath = Path;

                    byte[] FilenameBuffer = new byte[1024];

                    byte[] FilePathBytes = Encoding.Default.GetBytes(FilePath);

                    Buffer.BlockCopy(FilePathBytes, 0, FilenameBuffer, 0, FilePathBytes.Length);

                    float Width = ImGui.GetContentRegionAvailWidth();
                    ImGui.PushItemWidth(Width);
                    ImGui.InputText("FileName", FilenameBuffer, (uint)FilenameBuffer.Length);
                    ImGui.PopItemWidth();

                    if (ImGui.Button("Ok"))
                    {
                        if (FileSelect)
                        {
                            WindowOpen = string.IsNullOrEmpty(FilePath);
                            Result     = !string.IsNullOrEmpty(FilePath);
                        }
                        else
                        {
                            WindowOpen = false;
                            Result     = true;
                        }
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
                    if (ImGui.MenuItem("Open ROM", !MIPS.Cores.R4300.R4300_ON))
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

                if (ImGui.BeginMenu("State"))
                {
                    string Paused = Common.Variables.Pause ? " (Paused)" : "";
                    if (ImGui.MenuItem($"Pause/Resume{Paused}"))
                    {
                        Common.Variables.Pause = !Common.Variables.Pause;
                    }

                    if (ImGui.MenuItem("Step", "F1", false, Common.Variables.Pause && MIPS.Cores.R4300.R4300_ON))
                    {
                        Common.Variables.Step = true;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.BeginMenu("Debugger", MIPS.Cores.R4300.R4300_ON))
                    {
                        if (ImGui.MenuItem("Register Viewer"))
                            WindowOpenState[3] = true;

                        if (ImGui.MenuItem("Memory Viewer"))
                            WindowOpenState[5] = true;

                        if (ImGui.MenuItem("TLB Entries"))
                            WindowOpenState[10] = true;

                        if (ImGui.MenuItem("Disassembler"))
                            WindowOpenState[11] = true;
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

        private static void ReloadRoms()
        {
            if (!string.IsNullOrEmpty(RomDirectory))
                Roms = Directory.EnumerateFiles(RomDirectory);
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
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, 0xFF404040);

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

            ImGui.PushStyleColor(ImGuiCol.ChildBg, 0xFF202020);

            SubmitMenuBar();

            if (WindowOpenState[0])
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
                ImGui.SetNextWindowSize(new Vector2(Win.Width, Win.Height - 19.0f));
                ImGui.SetNextWindowPos(new Vector2(0, 19.0f));
                if (ImGui.Begin("##Games", ref WindowOpenState[0], ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBringToFrontOnFocus))
                {
                    ImGui.Text("Game List");
                    ImGui.Separator();
                    if (ImGui.BeginChild("##Gamelist"))
                    {
                        if (!string.IsNullOrEmpty(RomDirectory))
                        {
                            foreach (string Rom in Roms)
                            {
                                if ((Path.GetExtension(Rom).ToUpper() == ".Z64" 
                                  || Path.GetExtension(Rom).ToUpper() == ".N64")
                                    && ImGui.Selectable(Path.GetFileName(Rom), false, (MIPS.Cores.R4300.R4300_ON) ? ImGuiSelectableFlags.Disabled : 0))
                                {
                                    InitEmulator(Rom);
                                }
                            }
                        }
                        else
                        {
                            ImGui.Selectable("Go to the Settings window (Edit -> Settings) to select the ROM directory.");
                        }
                        ImGui.EndChild();
                    }
                    ImGui.End();
                }
                ImGui.PopStyleVar(1);
            }

            if (WindowOpenState[1])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(735, 464), new Vector2(2048, 2048));
                if (ImGui.Begin("Settings", ref WindowOpenState[1], ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Text("GUI Settings");
                    ImGui.Separator();

                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                        ImGui.Checkbox("Hide all directories and files starting with \".\"", ref HideDotFilesOnUnix);

                    byte[] RomDirBuf = new byte[1024];

                    byte[] RomDirectoryBytes = Encoding.Default.GetBytes(RomDirectory);

                    Buffer.BlockCopy(RomDirectoryBytes, 0, RomDirBuf, 0, RomDirectoryBytes.Length);

                    ImGui.InputText("ROM Directory", RomDirBuf, (uint)RomDirBuf.Length, ImGuiInputTextFlags.ReadOnly);

                    if (ImGui.Button("Choose ROM Directory"))
                        WindowOpenState[8] = true;

                    if (ImGui.Button("Reload Game List"))
                        ReloadRoms();

                    ImGui.Text("Emulator Settings");
                    ImGui.Separator();
                    ImGui.Checkbox("Debug Logging",        ref Common.Variables.Debug);
                    ImGui.Checkbox("Use Graphics LLE",     ref GraphicsLLE);
                    ImGui.Checkbox("Enable Expansion Pak", ref ExpansionPak);

                    if (ImGui.Button("Apply"))
                        SaveIni();
                    ImGui.End();
                }
            }

            if (WindowOpenState[3])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(256, 550), new Vector2(2048, 2048));
                if (ImGui.Begin("Register Viewer", ref WindowOpenState[3]))
                {
                    ImGui.Text($"R4300:");
                    ImGui.Text($" PC: 0x{MIPS.Registers.R4300.PC:X8}");
                    ImGui.Text($" HI: 0x{MIPS.Registers.R4300.HI:X16}");
                    ImGui.Text($" LO: 0x{MIPS.Registers.R4300.LO:X16}");
                    ImGui.Text($" LL: 0x{MIPS.Registers.R4300.LLbit:X1}");
                    for (uint i = 0; i < MIPS.Registers.R4300.Reg.Length; ++i)
                        ImGui.Text($" R[{i}]: 0x{MIPS.Registers.R4300.Reg[i]:X16}");
                    ImGui.Separator();
                    ImGui.Text($"COP0:");
                    for (uint i = 0; i < MIPS.Registers.COP0.Reg.Length; ++i)
                        ImGui.Text($" COP0R[{i}]: 0x{MIPS.Registers.COP0.Reg[i]:X16}");
                    ImGui.Separator();
                    ImGui.Text($"RSP: (Halted = {MIPS.Cores.RSP.RSP_HALT})");
                    ImGui.Text($" PC: 0x{MIPS.Registers.RSPReg.PC:X3}");
                    for (uint i = 0; i < MIPS.Registers.RSPReg.Reg.Length; ++i)
                        ImGui.Text($" RSPR[{i}]: 0x{MIPS.Registers.RSPReg.Reg[i]:X8}");
                    ImGui.Separator();
                    ImGui.Text($"RSPCOP0: (Halted = {MIPS.Cores.RSP.RSP_HALT})");
                    for (uint i = 0; i < MIPS.Registers.RSPCOP0.Reg.Length; ++i)
                        ImGui.Text($" RSPC0R[{i}]: 0x{MIPS.Registers.RSPCOP0.Reg[i]:X16}");
                    ImGui.Separator();
                    ImGui.Text($"RSPCOP2: (Halted = {MIPS.Cores.RSP.RSP_HALT})");
                    for (uint i = 0; i < MIPS.Registers.RSPCOP2.Reg.Length; ++i)
                    {
                        ImGui.Text($" RSPC2R[{i}]: ");
                        for (uint j = 0; j < 8; ++j)
                        {
                            ImGui.SameLine();
                            if (j == 7) ImGui.Text($"{MIPS.Registers.RSPCOP2.Reg[i].GetElement(j * 2):X4}");
                            else ImGui.Text($"{MIPS.Registers.RSPCOP2.Reg[i].GetElement(j * 2):X4}|");
                        }
                    }

                    ImGui.End();
                }
            }

            if (WindowOpenState[10])
            {
                if (ImGui.Begin("TLB Entries", ref WindowOpenState[10], ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Text("TLB Entries");
                    ImGui.Separator();

                    for (uint i = 0; i < MIPS.TLB.TLBEntries.Length; ++i)
                    {
                        MIPS.TLB.TLBEntry Entry = MIPS.TLB.TLBEntries[i];
                        ImGui.Text($"Entry {i}:");
                        ImGui.Text($" EntryHi:  0x{Entry.EntryHi:X8}");
                        ImGui.Text($" PageMask: 0x{Entry.PageMask:X8}");
                        ImGui.Text($" Even:");
                        ImGui.Text($"  PFN:           0x{Entry.PFN0:X8}");
                        ImGui.Text($"  PageCoherency: 0x{Entry.PageCoherency0:X2}");
                        ImGui.Text($"  Valid:         0x{Entry.Valid0:X2}");
                        ImGui.Text($"  Global:        0x{Entry.Global0:X2}");
                        ImGui.Text($" Odd:");
                        ImGui.Text($"  PFN:           0x{Entry.PFN1:X8}");
                        ImGui.Text($"  PageCoherency: 0x{Entry.PageCoherency1:X2}");
                        ImGui.Text($"  Valid:         0x{Entry.Valid1:X2}");
                        ImGui.Text($"  Global:        0x{Entry.Global1:X2}");
                        ImGui.Separator();
                    }

                    ImGui.End();
                }
            }

            if (WindowOpenState[11])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(570, 430), new Vector2(2048, 2048));
                if (ImGui.Begin("Disassembler", ref WindowOpenState[11], ImGuiWindowFlags.NoCollapse))
                {
                    if (Disasm_AddrBuf[0] == 0)
                    {
                        string AddrStr = "00000000";

                        byte[] AddrStrBytes = Encoding.Default.GetBytes(AddrStr);

                        Buffer.BlockCopy(AddrStrBytes, 0, Disasm_AddrBuf, 0, AddrStrBytes.Length);
                    }

                    ImGui.Text("Address: 0x");
                    ImGui.SameLine();
                    ImGui.InputText("##Address", Disasm_AddrBuf, (uint)Disasm_AddrBuf.Length, ImGuiInputTextFlags.CharsHexadecimal);

                    ImGui.SameLine();
                    if (ImGui.Button("Go"))
                        Disasm_CurrAddr = uint.Parse(Encoding.Default.GetString(Disasm_AddrBuf).Split('\0')[0], NumberStyles.HexNumber);


                    for (uint i = 0; i < 128; ++i)
                    {
                        byte[] Inst = new byte[4];
                        uint InstAddr = Disasm_CurrAddr + (i * 4);
                        Inst = MIPS.Cores.R4300.memory[InstAddr, 4, false];
                        uint InstInt = (uint)(Inst[3] | (Inst[2] << 8) | (Inst[1] << 16) | (Inst[0] << 24));
                        try
                        {
                            MIPS.OpcodeTable.InstInfo   Info = MIPS.OpcodeTable.GetOpcodeInfo (InstInt, true, true);
                            MIPS.OpcodeTable.OpcodeDesc Desc = new MIPS.OpcodeTable.OpcodeDesc(InstInt, true, true);
                            if (MIPS.Registers.R4300.PC == InstAddr && Common.Variables.Pause) ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                            ImGui.Text($"0x{InstAddr:X8}: {string.Format(Info.FormattedASM, Desc.op1, Desc.op2, Desc.op3, Desc.op4, Desc.Imm, Desc.Target, Desc.VOff, Desc.Ve)}");
                            if (MIPS.Registers.R4300.PC == InstAddr && Common.Variables.Pause) ImGui.PopStyleColor();
                        }
                        catch (Exception)
                        {
                            ImGui.Text($"$0x{InstAddr:X8}: UNKNOWN");
                        }
                    }

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

            if (WindowOpenState[5] && !MIPS.Cores.R4300.R4300_ON) WindowOpenState[5] = false;
            MemoryViewer(ref MemoryViewer_CurrAddr, ref WindowOpenState[5], ref MemoryViewer_AddrBuf, "Memory Viewer");

            if (FileBrowser(ref FileDialogRom_CurrPath, ref FileDialogRom_FilePath, 
                ref WindowOpenState[2], ref WindowOpenState[6], 
                ref HideDotFilesOnUnix, "Select a ROM to load.", true))
            {
                InitEmulator(FileDialogRom_FilePath);

                FileDialogRom_FilePath = "";
            }

            if (FileBrowser(ref FolderDialogRoms_CurrPath, ref FolderDialogRoms_FilePath, 
                ref WindowOpenState[8], ref WindowOpenState[9], 
                ref HideDotFilesOnUnix, "Select ROM Directory.", false))
            {
                RomDirectory = FolderDialogRoms_FilePath;
                ReloadRoms();
            }

            if (WindowOpenState[7])
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(256, 64), new Vector2(2048, 2048));
                if (ImGui.Begin("ROM Error", ref WindowOpenState[7], ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
                {
                    ImGui.Text("This ROM is either a bad ROM or it is little endian (byte swapping not implemented yet)");
                    ImGui.End();
                }
            }

            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
        }
    }
}
