using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiFontMap
{
    public class GuiFontMapTable : ITable
    {
        public List<GuiFontMapItem> Items { get; init; } = null!;

        private GuiFontMapTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new GuiFontMapItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                int id = i;
                string alias = reader.ReadStringBuffer(32);
                string name = reader.ReadStringBuffer(32);
                var flags = (EFontMapFlags)reader.ReadUInt32();
                float scale = reader.ReadSingle();
                float offsetX = reader.ReadSingle();
                float offsetY = reader.ReadSingle();
                var platform = (EFontMapPlatform)reader.ReadUInt32();
                var language = (EFontMapLanguage)reader.ReadUInt32();
                
                items[i] = new GuiFontMapItem()
                {
                    Id = id,
                    Alias = alias,
                    Name = name,
                    Flags = flags,
                    Scale = scale,
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                    Platform = platform,
                    Language = language
                };
            }

            return new GuiFontMapTable()
            {
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            for (var i = 0; i < itemsCount; i++)
            {
                GuiFontMapItem item = Items[i];
                writer.WriteStringBuffer(32, item.Alias);
                writer.WriteStringBuffer(32, item.Name);
                writer.Write((uint)item.Flags);
                writer.Write(item.Scale);
                writer.Write(item.OffsetX);
                writer.Write(item.OffsetY);
                writer.Write((uint)item.Platform);
                writer.Write((uint)item.Language);
            }
        }
    }
}
