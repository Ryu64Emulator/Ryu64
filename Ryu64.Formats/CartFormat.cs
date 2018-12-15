namespace Ryu64.Formats
{
    public class CartFormat
    {
        public string Path;

        public CartFormat(string Path)
        {
            this.Path = Path;
        }

        public byte[] AllData;
        public string Name;

        public bool HasBeenParsed = false;

        public virtual void Parse()
        {
        }
    }
}
