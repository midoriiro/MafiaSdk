namespace Core.IO.ResourceFormats.XBin.Types.GameMeshBinding
{
    public class GameMeshBindingTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public List<GameMeshBindingItem> Items { get; init; } = null!;

        private GameMeshBindingTable()
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
            
            var items = new GameMeshBindingItem[count0];

            for (var i = 0; i < count1; i++)
            {
                ulong nameHash = reader.ReadUInt64();
                ulong singleMeshIndex = reader.ReadUInt64();
                ulong havokIndex = reader.ReadUInt64();

                items[i] = new GameMeshBindingItem()
                {
                    NameHash = nameHash,
                    SingleMeshIndex = singleMeshIndex,
                    HavokIndex = havokIndex
                };
            }

            uint unk1 = reader.ReadUInt32();

            return new GameMeshBindingTable()
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

            foreach(GameMeshBindingItem bind in Items)
            {
                writer.Write(bind.NameHash);
                writer.Write(bind.SingleMeshIndex);
                writer.Write(bind.HavokIndex);
            }

            writer.Write(Unk1);
        }
    }
}
