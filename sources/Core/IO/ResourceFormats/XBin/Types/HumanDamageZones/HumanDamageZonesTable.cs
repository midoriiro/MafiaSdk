using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.HumanDamageZones
{
    public partial class HumanDamageZonesTable : ITable
    {
        public int Unk0 { get; init; }
        public int Unk1 { get; init; }
        public List<HumanDamageZoneItem> Items { get; init; } = null!;

        private HumanDamageZonesTable()
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
            
            int unk1 = reader.ReadInt32();
            var items = new HumanDamageZoneItem[count1];
            
            for (var i = 0; i < items.Length; i++)
            {
                items[i] = HumanDamageZoneItem.ReadFromFile(reader);
            }

            foreach (HumanDamageZoneItem item in items)
            {
                for (var x = 0; x < item.StunGroups.Length; x++)
                {
                    item.StunGroups[x] = StunGroup.ReadFromFile(reader);
                }

                foreach (StunGroup stunGroup in item.StunGroups)
                {
                    for(var z = 0; z < stunGroup.BodyPartList.Length; z++)
                    {
                        stunGroup.BodyPartList[z] = (EBodyPartType)reader.ReadUInt32();
                    }
                }
            }

            return new HumanDamageZonesTable()
            {
                Unk0 = unk0,
                Unk1 = unk1,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            // TODO write to file
            for (var i = 0; i < itemsCount; i++)
            {
            }
        }
    }
}
