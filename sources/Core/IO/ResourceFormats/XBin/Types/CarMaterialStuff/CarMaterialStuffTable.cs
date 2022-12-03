using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.CarMaterialStuff
{
    public class CarMaterialStuffTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarMaterialStuffItem> Items { get; init; } = null!;

        private CarMaterialStuffTable()
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
            
            var items = new CarMaterialStuffItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string materialName = reader.ReadStringBuffer(32);
                var flags = (ECarMtrStuffFlags)reader.ReadUInt32();
                float dirtSpeedMin = reader.ReadSingle();
                float dirtSpeedMax = reader.ReadSingle();
                float dirtCoefficient = reader.ReadSingle();
                float temperaturePercentCoefficient = reader.ReadSingle();
                
                items[i] = new CarMaterialStuffItem()
                {
                    Id = id,
                    MaterialName = materialName,
                    Flags = flags,
                    DirtSpeedMin = dirtSpeedMin,
                    DirtSpeedMax = dirtSpeedMax,
                    DirtCoefficient = dirtCoefficient,
                    TemperaturePercentCoefficient = temperaturePercentCoefficient
                };
            }

            return new CarMaterialStuffTable()
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
                CarMaterialStuffItem item = Items[i];
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.MaterialName);
                writer.Write((uint)item.Flags);
                writer.Write(item.DirtSpeedMin);
                writer.Write(item.DirtSpeedMax);
                writer.Write(item.DirtCoefficient);
                writer.Write(item.TemperaturePercentCoefficient);
            }
        }
    }
}
