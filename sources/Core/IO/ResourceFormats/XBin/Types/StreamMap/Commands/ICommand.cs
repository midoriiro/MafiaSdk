namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public interface ICommand
    {
        uint Magic { get; }
        int Size { get; }
        int BytesToWrite => Size;
        
        static abstract ICommand ReadFromFile(BinaryReader reader);

        void WriteToFile(XBinWriter writer);
        void FixUpStringPointer(XBinWriter writer);
    }
}
