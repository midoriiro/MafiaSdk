using Core.Games;

namespace Core.IO.ResourceFormats.XBin.Types.Cities
{
    public class CitiesTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public GamesEnumerator GameVersion { get; init; }
        public List<CitiesTableItem> Items { get; init; } = null!;

        private CitiesTable()
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
            
            uint unk1 = reader.ReadUInt32();
            var items = new CitiesTableItem[count0];
            GamesEnumerator gameVersion = GameWorkSpace.Instance().SelectedGame.Type;

            for (var i = 0; i < count1; i++)
            {
                items[i] = CitiesTableItem.ReadFromFile(reader, gameVersion);
            }

            foreach(CitiesTableItem item in items)
            {
                item.ReadRespawnPlaces(reader);
            }

            return new CitiesTable()
            {
                Unk0 = unk0,
                Unk1 = unk1,
                GameVersion = gameVersion,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);
            writer.Write(Unk1);

            foreach(CitiesTableItem item in Items)
            {
                item.WriteToFile(writer, GameVersion);
            }

            foreach(CitiesTableItem item in Items)
            {
                item.WriteRespawnPlaces(writer);
            }

            foreach(CitiesTableItem item in Items)
            {
                writer.FixUpStringPointer(item.Name);
                writer.FixUpStringPointer(item.MissionLine);
                writer.FixUpStringPointer(item.SdsPrefix);
                
                if (GameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
                {
                    writer.FixUpStringPointer(item.Unk0!);
                }
                
                writer.FixUpStringPointer(item.Map);
                
                foreach (CityRespawnPlace respawn in item.CityRespawnPlaces)
                {
                    writer.FixUpStringPointer(respawn.StreamMapPart);
                }
            }
            
            writer.FixUpStringPointers();
        }
    }
}
