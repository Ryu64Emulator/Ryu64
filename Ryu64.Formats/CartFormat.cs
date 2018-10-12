namespace Ryu64.Formats
{
    public class CartFormat
    {
        public string Path;

        public CartFormat(string Path)
        {
            this.Path = Path;
        }

        public struct Header
        {
            public uint   ProgramCounter;
            public string Name;
        }

        public Header header;
        public byte[] AllData;

        public bool HasBeenParsed = false;

        public virtual void Parse()
        {
        }
    }
}
