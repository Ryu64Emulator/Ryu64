using System;

namespace Ryu64.CLI
{
    public class ArgumentParser
    {
        public delegate void CLIMethod(string[] args, uint CurrentPosition);

        public struct CLIFlags
        {
            public string InputRom;
            public bool UTEsyscall;
            public bool Debug;
            public bool NoWindow;
            public MIPS.R4300.TVType_enum TVtype;
        }

        public struct CLIFlag
        {
            public string FlagName;
            public string Help;
            public uint Arguments;
            public CLIMethod Method;
        }

        public static CLIFlag[] FlagList =
        {
            new CLIFlag
            {
                FlagName = "debug",
                Help = "Turns on Debug mode.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.Debug = true;
                }
            },
            new CLIFlag
            {
                FlagName = "utesyscall",
                Help = "Enables the Unit Test Syscall, used for homebrew tests by Fraser.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.UTEsyscall = true;
                }
            },
            new CLIFlag
            {
                FlagName = "nowindow",
                Help = "Disables the Window.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.NoWindow = true;
                }
            },
            new CLIFlag
            {
                FlagName = "ntsc",
                Help = "Emulates a NTSC Nintendo 64. (This is the default)",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.TVtype = MIPS.R4300.TVType_enum.NTSC;
                }
            },
            new CLIFlag
            {
                FlagName = "pal",
                Help = "Emulates a PAL Nintendo 64.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.TVtype = MIPS.R4300.TVType_enum.PAL;
                }
            },
            new CLIFlag
            {
                FlagName = "mpal",
                Help = "Emulates a MPAL Nintendo 64.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.TVtype = MIPS.R4300.TVType_enum.MPAL;
                }
            },
            new CLIFlag
            {
                FlagName = "help",
                Help = "Displays this help message.",
                Arguments = 0,
                Method = (a, i) =>
                {
                    PrintHelp();
                    Environment.Exit(0);
                }
            }
        };

        public static CLIFlags Flags;

        private static void PrintHelp()
        {
            foreach (CLIFlag Flag in FlagList)
                Common.Logger.PrintInfo($"-{Flag.FlagName}:\n  {Flag.Help}\n", false);
        }

        private static void PrintUsage()
        {
            Common.Logger.PrintInfo("Usage: [Path to ROM] [Options]\n", false);
        }

        public static void ParseArguments(string[] args)
        {
            Flags.TVtype = MIPS.R4300.TVType_enum.NTSC;

            for (uint i = 0; i < args.Length; ++i)
            {
                if (args[i][0] == '-')
                {
                    foreach (CLIFlag Flag in FlagList)
                    {
                        if (args[i].Substring(1).ToUpper() == Flag.FlagName.ToUpper())
                        {
                            if ((args.Length - i) < Flag.Arguments)
                            {
                                Common.Logger.PrintError($"The Flag -{Flag.FlagName} needs {Flag.Arguments} arguments.\n", false);
                                Environment.Exit(1);
                            }

                            Flag.Method(args, i);
                            i += Flag.Arguments;
                            if (i > args.Length) break;
                        }
                    }
                }
                else
                {
                    Flags.InputRom = args[i];
                }
            }

            if (string.IsNullOrWhiteSpace(Flags.InputRom))
            {
                Common.Logger.PrintError("Please specify a ROM to load.\n", false);
                PrintUsage();
                PrintHelp();
                Environment.Exit(1);
            }

            Common.Variables.Debug      = Flags.Debug;
            Common.Variables.UTEsyscall = Flags.UTEsyscall;
        }
    }
}
