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
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Common.Logger.PrintErrorLine("Please specify a .z64 file to open.");
                Environment.Exit(-1);
            }

            Z64 Rom = new Z64(args[0]);
            Rom.Parse();

            if (!Rom.HasBeenParsed)
            {
                Common.Logger.PrintErrorLine("Can't open .z64, it's either, a bad .z64 or it is in Little Endian.");
                Environment.Exit(-1);
            }

            for (ulong i = 0x40, j = 0x0; i < 0x1000; ++i, ++j)
                MIPS.Memory.WriteUInt8(j, Rom.AllData[i]);

            MIPS.R4300.PowerOnR4300(Rom.Header.ProgramCounter);
        }
    }
}
