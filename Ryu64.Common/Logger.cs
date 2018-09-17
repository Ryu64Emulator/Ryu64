using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ryu64.Common
{
    public class Logger
    {
        public static string Log = "";

        public static void PrintInfo(string Info)
        {
            Log += Info;

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
            Log += Error;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Error);
            Console.ResetColor();
        }

        public static void PrintErrorLine(string Error)
        {
            PrintError(Error + '\n');
        }

        public static void PrintSuccess(string Success)
        {
            Log += Success;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Success);
            Console.ResetColor();
        }

        public static void PrintSuccessLine(string Success)
        {
            PrintSuccess(Success + '\n');
        }

        public static void DumpLog()
        {
            File.WriteAllText($"./Ryu64Log_{DateTime.Now.ToString("dddd_MM-dd-yyyy_HH-mm-ss_tt")}.log", Log);
        }
    }
}
