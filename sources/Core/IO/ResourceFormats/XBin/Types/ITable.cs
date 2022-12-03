namespace Core.IO.ResourceFormats.XBin.Types
{
    public interface ITable
    {
        static abstract ITable ReadFromFile(BinaryReader reader);
        void WriteToFile(XBinWriter writer);
    }
}
