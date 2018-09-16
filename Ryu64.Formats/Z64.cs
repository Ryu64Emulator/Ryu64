using Ryu64.Common;
using System;
using System.IO;

namespace Ryu64.Formats
{
    public class Z64
    {
        public string Path;

        public Z64(string Path)
        {
            this.Path = Path;
        }

        public struct Z64Header
        {
            public uint   ProgramCounter;
            public string Name;
        }

        public Z64Header Header;
        public byte[]    AllData;

        public bool HasBeenParsed = false;

        public void Parse()
        {
            FileStream Stream = File.OpenRead(Path);

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
                Header.ProgramCounter = BitConverter.ToUInt32(Reader.ReadBytes(8), 0);

                Stream.Position = 0x20;
                Header.Name = System.Text.Encoding.Default.GetString(Reader.ReadBytes(20));

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
