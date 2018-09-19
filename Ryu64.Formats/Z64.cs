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
            FileStream Stream = File.OpenRead(Path);
            header = new Header();

            using (BinaryReaderBE Reader = new BinaryReaderBE(Stream))
            {
                bool IsCorrectEndian = Reader.ReadUInt32() == 0x80371240;
                
                if (!IsCorrectEndian)
                {
                    Reader.Dispose();
                    Stream.Dispose();
                    return;
                }

                Stream.Position = 0x08;
                header.ProgramCounter = BitConverter.ToUInt32(Reader.ReadBytes(8), 0);

                Stream.Position = 0x20;
                header.Name = System.Text.Encoding.Default.GetString(Reader.ReadBytes(20));

                Stream.Position = 0x0;
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream.CopyTo(ms);
                    AllData = ms.ToArray();
                }

                HasBeenParsed = true;
            }

            Stream.Dispose();
        }
    }
}
