namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    // Essentially an empty command; holds no data.
    // (This is witnessed on M3.)
    public class Barrier : ICommand
    {
        public uint Magic { get; } = 0x31247C78;
        public int Size { get; } = 4;
        
        public uint Unk0 { get; init; }

        private Barrier()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();

            return new Barrier()
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
