using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.PaintCombinations
{
    public class PaintCombinationsTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<PaintCombinationsTableItem> Items { get; init; } = null!;

        private PaintCombinationsTable()
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
            
            var items = new PaintCombinationsTableItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                int id = reader.ReadInt32();
                int unk01 = reader.ReadInt32();
                int minOccurences = reader.ReadInt32();
                int maxOccurences = reader.ReadInt32();
                string carName = reader.ReadStringBuffer(32).Trim('\0');
                
                items[i] = new PaintCombinationsTableItem()
                {
                    Id = id,
                    Unk01 = unk01,
                    MinOccurences = minOccurences,
                    MaxOccurences = maxOccurences,
                    CarName = carName,
                    ColorIndex = new int[maxOccurences]
                };
            }

            foreach (PaintCombinationsTableItem item in items)
            {
                for (var z = 0; z < item.MaxOccurences; z++)
                {
                    item.ColorIndex[z] = reader.ReadInt32();
                }
            }

            return new PaintCombinationsTable()
            {
                Unk0 = unk0,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            for(var i = 0; i < itemsCount; i++)
            {
                PaintCombinationsTableItem item = Items[i];
                writer.Write(item.Id);
                writer.Write(item.Unk01);
                writer.Write(item.MinOccurences);
                writer.Write(item.MaxOccurences);
                writer.WriteString32(item.CarName);
            }

            for (var i = 0; i < itemsCount; i++)
            {
                PaintCombinationsTableItem item = Items[i];
                for (var z = 0; z < item.MaxOccurences; z++)
                {
                    writer.Write(item.ColorIndex[z]);
                }
            }
        }
    }
}
