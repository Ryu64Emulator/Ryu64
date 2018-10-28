using System;

namespace Ryu64.Common
{
    public class Logger
    {
        public static void PrintInfo(string Info)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(Info);
            Console.ResetColor();
        }

        public static void PrintInfoLine(string Info)
        {
            PrintInfo(Info + '\n');
        }

        public static void PrintError(string Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Error);
            Console.ResetColor();
        }

        public static void PrintErrorLine(string Error)
        {
            PrintError(Error + '\n');
        }

        public static void PrintWarning(string Warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(Warning);
            Console.ResetColor();
        }

        public static void PrintWarningLine(string Warning)
        {
            PrintWarning(Warning + '\n');
        }

        public static void PrintSuccess(string Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Success);
            Console.ResetColor();
        }

        public static void PrintSuccessLine(string Success)
        {
            PrintSuccess(Success + '\n');
        }
    }
}
