using System;

namespace Ryu64.Common
{
    public class Logger
    {
        private const string TimeFormatting = "mm\\:ss\\.ffff";

        public static void PrintInfo(string Info)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{Measure.MeasureTime.Elapsed.ToString(TimeFormatting)} | {Info}");
            Console.ResetColor();
        }

        public static void PrintInfoLine(string Info)
        {
            PrintInfo(Info + '\n');
        }

        public static void PrintError(string Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Measure.MeasureTime.Elapsed.ToString(TimeFormatting)} | {Error}");
            Console.ResetColor();
        }

        public static void PrintErrorLine(string Error)
        {
            PrintError(Error + '\n');
        }

        public static void PrintWarning(string Warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{Measure.MeasureTime.Elapsed.ToString(TimeFormatting)} | {Warning}");
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
