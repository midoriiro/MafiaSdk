using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.CarColors
{
    public class CarColorsTable : ITable
    {
        public int Unk0 { get; init; }
        public List<CarColorsItem> Items { get; init; } = null!;

        private CarColorsTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            int unk0 = reader.ReadInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();

            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new CarColorsItem[count0];

            for(var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string colorNameIndex = reader.ReadStringBuffer(8);
                byte red = reader.ReadByte();
                byte green = reader.ReadByte();
                byte blue = reader.ReadByte();
                string colorName = reader.ReadStringBuffer(16);
                string speechDarkLight = reader.ReadStringBuffer(32);
                string policeComm = reader.ReadStringBuffer(32);
                byte carColorsItemUnk0 = reader.ReadByte();
                
                items[i] = new CarColorsItem()
                {
                    Id = id,
                    ColorNameIndex = colorNameIndex,
                    Red = red,
                    Green = green,
                    Blue = blue,
                    ColorName = colorName,
                    SpeechDarkLight = speechDarkLight,
                    PoliceComm = policeComm,
                    Unk0 = carColorsItemUnk0
                };
            }

            return new CarColorsTable()
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
                CarColorsItem item = Items[i];
                writer.Write(item.Id);
                writer.WriteStringBuffer(8, item.ColorNameIndex);
                writer.Write(item.Red);
                writer.Write(item.Green);
                writer.Write(item.Blue);
                writer.WriteStringBuffer(16, item.ColorName);
                writer.WriteStringBuffer(32, item.SpeechDarkLight);
                writer.WriteStringBuffer(32, item.PoliceComm);
                writer.Write(item.Unk0);
            }
        }
    }
}
