namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    // Essentially an empty command; holds no data.
    // (This is witnessed on M3.)
    public class WaitForMovie : ICommand
    {
        public uint Magic { get; } = 0x665E90F2;
        public int Size { get; } = 4;
        public int BytesToWrite => Unk0 == 0 ? 1 : Size; 
        
        public uint Unk0 { get; init; }

        private WaitForMovie()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();

            return new WaitForMovie()
            {
                Unk0 = unk0
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write(Unk0);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
