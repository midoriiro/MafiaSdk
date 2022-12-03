namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class UnlockVehicle : ICommand
    {
        public uint Magic { get; } = 0x3B3DD38A;
        public int Size { get; } = 4;
        
        public uint Guid { get; init; }

        private UnlockVehicle()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            uint guid = reader.ReadUInt32();

            return new UnlockVehicle()
            {
                Guid = guid
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write(Guid);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
