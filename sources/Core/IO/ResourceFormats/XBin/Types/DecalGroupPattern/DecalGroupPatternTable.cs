namespace Core.IO.ResourceFormats.XBin.Types.DecalGroupPattern
{
    public class DecalGroupPatternTable : ITable
    {
        public List<DecalGroupPatternItem> Items { get; init; } = null!;

        private DecalGroupPatternTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new DecalGroupPatternItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                uint maxPc = reader.ReadUInt32();
                uint maxXbox = reader.ReadUInt32();
                uint maxPs3 = reader.ReadUInt32();
                float fadeOut = reader.ReadSingle();
                uint maxDistance = reader.ReadUInt32();
                
                items[i] = new DecalGroupPatternItem()
                {
                    Id = id,
                    MaxPc = maxPc,
                    MaxXbox = maxXbox,
                    MaxPs3 = maxPs3,
                    FadeOut = fadeOut,
                    MaxDistance = maxDistance
                };
            }

            return new DecalGroupPatternTable()
            {
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            for (var i = 0; i < itemsCount; i++)
            {
                DecalGroupPatternItem item = Items[i];
                writer.Write(item.Id);
                writer.Write(item.MaxPc);
                writer.Write(item.MaxXbox);
                writer.Write(item.MaxPs3);
                writer.Write(item.FadeOut);
                writer.Write(item.MaxDistance);
            }
        }
    }
}
