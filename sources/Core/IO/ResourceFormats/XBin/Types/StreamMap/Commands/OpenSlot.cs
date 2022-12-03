namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{ 
    public class OpenSlot : ICommand
    {
        public uint Magic { get; } = 0xD7C10363;
        public int Size { get; } = 8;
        
        public uint TypeId { get; init; }
        public uint SlotId { get; init; }

        private OpenSlot()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            uint typeId = reader.ReadUInt32();
            uint slotId = reader.ReadUInt32();

            return new OpenSlot()
            {
                TypeId = typeId,
                SlotId = slotId
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(SlotId);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
