namespace Core.IO.ResourceFormats.XBin.Types.CharacterCinematics
{
    public class CharacterCinematicsTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public List<CharacterCinematicsItem> Items { get; init; } = null!;

        private CharacterCinematicsTable()
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
            
            var items = new CharacterCinematicsItem[count0];

            for (var i = 0; i < count1; i++)
            {
                ulong stringId = reader.ReadUInt64();
                ulong characterId = reader.ReadUInt64();

                items[i] = new CharacterCinematicsItem()
                {
                    StringId = stringId,
                    CharacterId = characterId
                };
            }
            
            uint unk1 = reader.ReadUInt32();

            return new CharacterCinematicsTable()
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

            foreach (CharacterCinematicsItem characterCinematic in Items)
            {
                writer.Write(characterCinematic.StringId);
                writer.Write(characterCinematic.CharacterId);
            }
            
            writer.Write(Unk1);
        }
    }
}
