using Ryu64.Common;
using System;
using System.IO;

namespace Ryu64.Formats
{
    public class Z64 : CartFormat
    {
        public Z64(string Path) : base(Path)
        {
        }

        public override void Parse()
        {
            AllData = File.ReadAllBytes(Path);

            bool IsCorrectEndian = BitConverter.ToUInt32(AllData, 0) == 0x40123780;

            if (!IsCorrectEndian)
                return;

            Name = "";

            for (int i = 0; i < 27; ++i)
                Name += (char)AllData[i + 0x20];

            HasBeenParsed = true;
        }
    }
}
