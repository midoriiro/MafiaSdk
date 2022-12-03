using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.CarWindowsTint
{
    public class CarWindowTintTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarWindowTintItem> Items { get; init; } = null!;

        private CarWindowTintTable()
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
            
            var items = new CarWindowTintItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string description = reader.ReadStringBuffer(32);
                byte red = reader.ReadByte();
                byte green = reader.ReadByte();
                byte blue = reader.ReadByte();
                byte alpha = reader.ReadByte();
                
                items[i] = new CarWindowTintItem()
                {
                    Id = id,
                    Description = description,
                    Red = red,
                    Green = green,
                    Blue = blue,
                    Alpha = alpha
                };
            }

            return new CarWindowTintTable()
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

            for (var i = 0; i < itemsCount; i++)
            {
                CarWindowTintItem item = Items[i];
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.Description);
                writer.Write(item.Red);
                writer.Write(item.Green);
                writer.Write(item.Blue);
                writer.Write(item.Alpha);
            }
        }
    }
}
