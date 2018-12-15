using Ryu64.Formats;
using System;
using System.IO;

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org/>
*/

namespace Ryu64
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CLI.ParseArguments(args);

            Z64 Rom = new Z64(CLI.Flags.InputRom);
            Rom.Parse();

            if (!Rom.HasBeenParsed)
            {
                Common.Logger.PrintErrorLine("Can't open ROM, it's either, a bad ROM or it is in Little Endian (Byte swapping not implemented yet).");
                Environment.Exit(1);
            }

            if (!Directory.Exists(Common.Variables.AppdataFolder))
            {
                Directory.CreateDirectory(Common.Variables.AppdataFolder);
                Directory.CreateDirectory($"{Common.Variables.AppdataFolder}/saves");
            }

            Common.Settings.Parse($"{AppDomain.CurrentDomain.BaseDirectory}/Settings.ini");

            MIPS.R4300.memory = new MIPS.Memory(Rom.AllData);

            MIPS.R4300.PowerOnR4300();

            if (!CLI.Flags.NoWindow) using (Graphics.MainWindow Window = new Graphics.MainWindow(Rom.Name.Trim())) Window.Run(60.0);
        }
    }
}
