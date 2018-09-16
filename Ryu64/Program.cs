using Ryu64.Formats;
using System;
using System.IO;

namespace Ryu64
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please specify a .z64 file to open.");
                Environment.Exit(-1);
            }

            Z64 Test = new Z64(args[0]);
            Test.Parse();

            if (!Test.HasBeenParsed)
            {
                Console.WriteLine("Can't open .z64, it's either, a bad .z64 or it is in Little Endian.");
                Environment.Exit(-1);
            }

            foreach (byte b in Test.AllData)
            {
                Console.WriteLine($"0x{b:x2}");
            }

            MIPS.R4300.PowerOnR4300();

            while (true);
        }
    }
}
