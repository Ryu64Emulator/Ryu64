using System;
using System.IO;

namespace Ryu64
{
    public class CLI
    {
        public delegate void CLIMethod(string[] args, uint CurrentPosition);

        public struct CLIFlags
        {
            public string InputRom;
            public bool UTEsyscall;
            public bool Debug;
            public bool NoWindow;
        }

        public struct CLIFlag
        {
            public string FlagName;
            public uint Arguments;
            public CLIMethod Method;
        }

        public static CLIFlag[] FlagList =
        {
            new CLIFlag
            {
                FlagName = "debug",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.Debug = true;
                }
            },
            new CLIFlag
            {
                FlagName = "utesyscall",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.UTEsyscall = true;
                }
            },
            new CLIFlag
            {
                FlagName = "nowindow",
                Arguments = 0,
                Method = (a, i) =>
                {
                    Flags.NoWindow = true;
                }
            }
        };

        public static CLIFlags Flags;

        public static void ParseArguments(string[] args)
        {
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
                                Common.Logger.PrintErrorLine($"The Flag -{Flag.FlagName} needs {Flag.Arguments} arguments");
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
                Common.Logger.PrintErrorLine("Please specify a ROM to load.");
                Environment.Exit(1);
            }

            Common.Variables.Debug      = Flags.Debug;
            Common.Variables.UTEsyscall = Flags.UTEsyscall;
        }
    }
}
