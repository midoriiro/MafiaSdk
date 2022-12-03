using Core.Games;
using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Missions
{
    public class MissionsTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public GamesEnumerator GameVersion { get; init; }
        public List<MissionItem> Items { get; init; } = null!;

        private MissionsTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            uint unk1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }

            var items = new MissionItem[count0];
            GamesEnumerator gameVersion = GameWorkSpace.Instance().SelectedGame.Type;

            for (var i = 0; i < count1; i++)
            {
                XBinHash64Name id = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name textId = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name descriptionId = XBinHash64Name.ReadFromFile(reader);
                uint iconId = reader.ReadUInt32();
                uint cityId = reader.ReadUInt32();
                var type = (EMissionType)reader.ReadUInt32();
                string missionId = reader.ReadStringPointerWithOffset();

                string? checkPointFile = null;
                uint? unknown = null;
                
                if (gameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
                {
                    checkPointFile = reader.ReadStringPointerWithOffset();
                    unknown = reader.ReadUInt32();
                }

                items[i] = new MissionItem()
                {
                    Id = id,
                    TextId = textId,
                    DescriptionId = descriptionId,
                    IconId = iconId,
                    CityId = cityId,
                    Type = type,
                    MissionId = missionId,
                    CheckPointFile = checkPointFile,
                    Unknown = unknown,
                };
            }

            return new MissionsTable()
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

            foreach (MissionItem item in Items)
            {
                item.Id.WriteToFile(writer);
                item.TextId.WriteToFile(writer);
                item.DescriptionId.WriteToFile(writer);
                writer.Write(item.IconId);
                writer.Write(item.CityId);
                writer.Write((int)item.Type);
                writer.PushStringPointer(item.MissionId);

                if (GameVersion != GamesEnumerator.Mafia1DefinitiveEdition)
                {
                    continue;
                }

                writer.PushStringPointer(item.CheckPointFile!);
                writer.Write(item.Unknown!.Value);
            }
            
            writer.FixUpStringPointers();
        }
    }
}
