using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.HumanMaterials
{
    public class HumanMaterialsTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<HumanMaterialsItem> Items { get; init; } = null!;

        private HumanMaterialsTable()
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

            var items = new HumanMaterialsItem[count1];
            
            for(var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string materialName = reader.ReadStringBuffer(32);
                var flags = (EHumanMaterialsTableItemFlags)reader.ReadUInt32();
                XBinHash32Name soundMaterialSwitch = XBinHash32Name.ReadFromFile(reader);
                uint stepParticles = reader.ReadUInt32();
                
                items[i] = new HumanMaterialsItem()
                {
                    Id = id,
                    MaterialName = materialName,
                    Flags = flags,
                    SoundMaterialSwitch = soundMaterialSwitch,
                    StepParticles = stepParticles
                };
            }

            return new HumanMaterialsTable()
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

            foreach(HumanMaterialsItem item in Items)
            {
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.MaterialName);
                writer.Write((uint)item.Flags);
                item.SoundMaterialSwitch.WriteToFile(writer);
                writer.Write(item.StepParticles);
            }
        }
    }
}
