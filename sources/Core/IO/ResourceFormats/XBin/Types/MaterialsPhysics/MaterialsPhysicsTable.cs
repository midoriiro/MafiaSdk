using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.MaterialsPhysics
{
    public class MaterialsPhysicsTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<MaterialsPhysicsItem> Items { get; init; } = null!;

        private MaterialsPhysicsTable()
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

            var items = new MaterialsPhysicsItem[count0];
            
            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string materialName = reader.ReadStringBuffer(32);
                XBinHash32Name soundSwitch = XBinHash32Name.ReadFromFile(reader);
                uint guidPart0 = reader.ReadUInt32();
                uint guidPart1 = reader.ReadUInt32();
                float staticFriction = reader.ReadSingle();
                float dynamicFriction = reader.ReadSingle();
                float restitution = reader.ReadSingle();
                string technicalNote = reader.ReadStringBuffer(64);
                
                items[i] = new MaterialsPhysicsItem()
                {
                    Id = id,
                    MaterialName = materialName,
                    SoundSwitch = soundSwitch,
                    GuidPart0 = guidPart0,
                    GuidPart1 = guidPart1,
                    StaticFriction = staticFriction,
                    DynamicFriction = dynamicFriction,
                    Restitution = restitution,
                    TechnicalNote = technicalNote
                };
            }

            return new MaterialsPhysicsTable()
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

            foreach (MaterialsPhysicsItem item in Items)
            {
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.MaterialName);
                item.SoundSwitch.WriteToFile(writer);
                writer.Write(item.GuidPart0);
                writer.Write(item.GuidPart1);
                writer.Write(item.StaticFriction);
                writer.Write(item.DynamicFriction);
                writer.Write(item.Restitution);
                writer.WriteStringBuffer(64, item.TechnicalNote);
            }
        }
    }
}
