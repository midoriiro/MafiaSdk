using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.CarSkidmarks
{
    public class CarSkidmarksTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarSkidmarksItem> Items { get; init; } = null!;

        private CarSkidmarksTable()
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
            
            var items = new CarSkidmarksItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string materialName = reader.ReadStringBuffer(32);
                int skidId = reader.ReadInt32();
                float skidAlpha = reader.ReadSingle();
                int rideId = reader.ReadInt32();
                float rideAlpha = reader.ReadSingle();
                float terrainDeep = reader.ReadSingle();
                float fadeTime = reader.ReadSingle();
                
                items[i] = new CarSkidmarksItem()
                {
                    Id = id,
                    MaterialName = materialName,
                    SkidId = skidId,
                    SkidAlpha = skidAlpha,
                    RideId = rideId,
                    RideAlpha = rideAlpha,
                    TerrainDeep = terrainDeep,
                    FadeTime = fadeTime
                };
            }

            return new CarSkidmarksTable()
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
                CarSkidmarksItem item = Items[i];
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.MaterialName);
                writer.Write(item.SkidId);
                writer.Write(item.SkidAlpha);
                writer.Write(item.RideId);
                writer.Write(item.RideAlpha);
                writer.Write(item.TerrainDeep);
                writer.Write(item.FadeTime);
            }
        }
    }
}
