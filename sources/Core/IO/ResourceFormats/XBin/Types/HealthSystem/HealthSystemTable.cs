namespace Core.IO.ResourceFormats.XBin.Types.HealthSystem
{
    public partial class HealthSystemTable : ITable
    {
        public uint Unk0 { get; init; } // 16
        public uint Unk1 { get; init; } // 0
        public List<HealthSystemItem> Items { get; init; } = null!; // TODO refactor all table properties to 'items'

        private HealthSystemTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            uint unk1 = reader.ReadUInt32();
            
            var items = new HealthSystemItem[count0];
            
            for(var i = 0; i < items.Length; i++)
            {
                items[i] = HealthSystemItem.ReadFromFile(reader);
            }

            foreach (HealthSystemItem item in items)
            {
                item.ReadHealthSegments(reader);
            }

            return new HealthSystemTable()
            {
                Unk0 = unk0,
                Unk1 = unk1,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            // TODO write to file
            throw new NotImplementedException();
        }
    }
}
