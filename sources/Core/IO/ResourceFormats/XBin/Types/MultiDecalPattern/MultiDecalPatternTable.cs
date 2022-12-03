using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.MultiDecalPattern
{
    public class MultiDecalPatternTable : ITable
    {
        public List<MultiDecalPatternItem> Items { get; init; } = null!;

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
            
            var items = new MultiDecalPatternItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                float probability = reader.ReadSingle();
                var flags = (EMultiDecalFlags)reader.ReadUInt32();
                uint numDecals = reader.ReadUInt32();
                float maxRightShift = reader.ReadSingle();
                float maxUpShift = reader.ReadSingle();
                float scaleFactor = reader.ReadSingle();
                float scaleRand = reader.ReadSingle();
                
                items[i] = new MultiDecalPatternItem()
                {
                    Id = id,
                    Probability = probability,
                    Flags = flags,
                    NumDecals = numDecals,
                    MaxRightShift = maxRightShift,
                    MaxUpShift = maxUpShift,
                    ScaleFactor = scaleFactor,
                    ScaleRand = scaleRand
                };
            }

            return new MultiDecalPatternTable()
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
                MultiDecalPatternItem item = Items[i];
                writer.Write(item.Id);
                writer.Write(item.Probability);
                writer.Write((uint)item.Flags);
                writer.Write(item.NumDecals);
                writer.Write(item.MaxRightShift);
                writer.Write(item.MaxUpShift);
                writer.Write(item.ScaleFactor);
                writer.Write(item.ScaleRand);
            }
        }
    }
}
