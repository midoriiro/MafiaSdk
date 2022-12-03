using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.Subtitle
{
    public class SubtitleTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public List<SubtitleTableItem> Items { get; init; } = null!;

        private SubtitleTable()
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

            var items = new SubtitleTableItem[count0];
            uint unk1 = reader.ReadUInt32();

            for (var i = 0; i < count1; i++)
            {
                XBinHash32Name subtitleId = XBinHash32Name.ReadFromFile(reader);
                XBinHash32Name soundId = XBinHash32Name.ReadFromFile(reader);
                XBinHash64Name facialAnimationName = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name longStringId = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name shortStringId = XBinHash64Name.ReadFromFile(reader);
                uint soundPreset = reader.ReadUInt32();
                XBinHash32Name voicePresetOverride = XBinHash32Name.ReadFromFile(reader);
                uint subtitlePriorityOverride = reader.ReadUInt32();
                uint subtitleItemUnk0 = reader.ReadUInt32();
                XBinHash64Name subtitleCharacter = XBinHash64Name.ReadFromFile(reader);
                
                items[i] = new SubtitleTableItem()
                {
                    SubtitleId = subtitleId,
                    SoundId = soundId,
                    FacialAnimationName = facialAnimationName,
                    LongStringId = longStringId,
                    ShortStringId = shortStringId,
                    SoundPreset = soundPreset,
                    VoicePresetOverride = voicePresetOverride,
                    SubtitlePriorityOverride = subtitlePriorityOverride,
                    Unk0 = subtitleItemUnk0,
                    SubtitleCharacter = subtitleCharacter
                };
            }

            return new SubtitleTable()
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
            writer.Write(Unk1);

            foreach (SubtitleTableItem item in Items)
            {
                writer.Write(item.SubtitleId.Hash);
                writer.Write(item.SoundId.Hash);
                writer.Write(item.FacialAnimationName.Hash);
                writer.Write(item.LongStringId.Hash);
                writer.Write(item.ShortStringId.Hash);
                writer.Write(item.SoundPreset);
                writer.Write(item.VoicePresetOverride.Hash);
                writer.Write(item.SubtitlePriorityOverride);
                writer.Write(item.Unk0);
                writer.Write(item.SubtitleCharacter.Hash);
            }
        }
    }
}
