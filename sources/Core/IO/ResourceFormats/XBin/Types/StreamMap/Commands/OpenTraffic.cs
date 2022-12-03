namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class OpenTraffic : ICommand
    {
        public uint Magic { get; } = 0xD4F4F264;
        public int Size { get; } = 4;
        
        public uint SeasonId { get; init; }

        private OpenTraffic()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            uint seasonId = reader.ReadUInt32();

            return new OpenTraffic()
            {
                SeasonId = seasonId
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write(SeasonId);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
