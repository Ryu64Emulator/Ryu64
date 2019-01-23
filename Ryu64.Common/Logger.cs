using System;
using System.Threading;

namespace Ryu64.Common
{
    public class Logger
    {
        private const string TimeFormatting = "mm\\:ss\\.ffff";

        public static void PrintInfo(string Info)
        {
            string ThreadName = (!string.IsNullOrEmpty(Thread.CurrentThread.Name)) ? $" | {Thread.CurrentThread.Name}" : "";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"Info: {Measure.MeasureTime.Elapsed.ToString(TimeFormatting)}{ThreadName} | {Info}");
            Console.ResetColor();
        }

        public static void PrintInfoLine(string Info)
        {
            PrintInfo(Info + '\n');
        }

        public static void PrintError(string Error)
        {
            string ThreadName = (!string.IsNullOrEmpty(Thread.CurrentThread.Name)) ? $" | {Thread.CurrentThread.Name}" : "";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Err:  {Measure.MeasureTime.Elapsed.ToString(TimeFormatting)}{ThreadName} | {Error}");
            Console.ResetColor();
        }

        public static void PrintErrorLine(string Error)
        {
            PrintError(Error + '\n');
        }

        public static void PrintWarning(string Warning)
        {
            string ThreadName = (!string.IsNullOrEmpty(Thread.CurrentThread.Name)) ? $" | {Thread.CurrentThread.Name}" : "";
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"Warn: {Measure.MeasureTime.Elapsed.ToString(TimeFormatting)}{ThreadName} | {Warning}");
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
