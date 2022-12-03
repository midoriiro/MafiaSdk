namespace Core.IO.ResourceFormats.XBin.Types.GenericSpeechSituations
{
    public class GenericSpeechSituationsTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public List<GenericSpeechSituationsItem> Items { get; init; } = null!;

        private GenericSpeechSituationsTable()
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
            
            var items = new GenericSpeechSituationsItem[count0];

            for (var i = 0; i < count1; i++)
            {
                ulong situationId = reader.ReadUInt64();
                uint cooldown = reader.ReadUInt32();
                uint perActorCooldownMin = reader.ReadUInt32();
                uint perActorCooldownMax = reader.ReadUInt32();
                uint perTeamCooldownMin = reader.ReadUInt32();
                uint perTeamCooldownMax = reader.ReadUInt32();
                uint isForeground = reader.ReadUInt32();
                ulong initHash = reader.ReadUInt64();

                items[i] = new GenericSpeechSituationsItem()
                {
                    SituationId = situationId,
                    Cooldown = cooldown,
                    PerActorCooldownMin = perActorCooldownMin,
                    PerActorCooldownMax = perActorCooldownMax,
                    PerTeamCooldownMin = perTeamCooldownMin,
                    PerTeamCooldownMax = perTeamCooldownMax,
                    IsForeground = isForeground,
                    InitHash = initHash
                };
            }

            uint unk1 = reader.ReadUInt32();

            return new GenericSpeechSituationsTable()
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

            foreach (GenericSpeechSituationsItem speechSituation in Items)
            {
                writer.Write(speechSituation.SituationId);
                writer.Write(speechSituation.Cooldown);
                writer.Write(speechSituation.PerActorCooldownMin);
                writer.Write(speechSituation.PerActorCooldownMax);
                writer.Write(speechSituation.PerTeamCooldownMin);
                writer.Write(speechSituation.PerTeamCooldownMax);
                writer.Write(speechSituation.IsForeground);
                writer.Write(speechSituation.InitHash);
            }

            writer.Write(Unk1);
        }
    }
}
