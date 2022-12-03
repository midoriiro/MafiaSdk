using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.MaterialsShots
{
    public class MaterialsShotsTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<MaterialsShotsItem> Items { get; init; } = null!;

        private MaterialsShotsTable()
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

            var items = new MaterialsShotsItem[count0];
            
            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string materialName = reader.ReadStringBuffer(32);
                uint guidPart0 = reader.ReadUInt32();
                uint guidPart1 = reader.ReadUInt32();
                XBinHash32Name soundSwitch = XBinHash32Name.ReadFromFile(reader);
                float penetration = reader.ReadSingle();
                uint particle = reader.ReadUInt32();
                uint decal = reader.ReadUInt32();
                uint decalCold = reader.ReadUInt32();
                
                items[i] = new MaterialsShotsItem()
                {
                    Id = id,
                    MaterialName = materialName,
                    GuidPart0 = guidPart0,
                    GuidPart1 = guidPart1,
                    SoundSwitch = soundSwitch,
                    Penetration = penetration,
                    Particle = particle,
                    Decal = decal,
                    DecalCold = decalCold
                };
            }

            return new MaterialsShotsTable()
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

            foreach (MaterialsShotsItem item in Items)
            {
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.MaterialName);
                writer.Write(item.GuidPart0);
                writer.Write(item.GuidPart1);
                item.SoundSwitch.WriteToFile(writer);
                writer.Write(item.Penetration);
                writer.Write(item.Particle);
                writer.Write(item.Decal);
                writer.Write(item.DecalCold);
            }
        }
    }
}
