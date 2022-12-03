using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.GuiLanguageMap
{
    public class GuiLanguageMapTable : ITable
    {
        public List<GuiLanguageMapItem> Items { get; init; } = null!;

        private GuiLanguageMapTable()
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
            
            var items = new GuiLanguageMapItem[count1];

            for (var index = 0; index < items.Length; index++)
            {
                string languageCode = reader.ReadStringBuffer(32);
                uint displayNameOffset = reader.ReadUInt32();
                int hasAudioLayer = reader.ReadInt32();
                
                items[index] = new GuiLanguageMapItem()
                {
                    LanguageCode = languageCode,
                    DisplayNameOffset = displayNameOffset,
                    HasAudioLayer = hasAudioLayer
                };
            }

            for (var i = 0; i < items.Length; i++)
            {
                GuiLanguageMapItem item = items[i];
                item.DisplayName = reader.ReadStringEncoded().TrimEnd('\0');
                items[i] = item;
            }

            return new GuiLanguageMapTable()
            {
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            var offsets = new long[itemsCount];
            
            for (var index = 0; index < itemsCount; index++)
            {
                GuiLanguageMapItem item = Items[index];
                writer.WriteStringBuffer(32, item.LanguageCode);
                offsets[index] = writer.BaseStream.Position;
                writer.Write(0xDEADC0DE); // placeholder
                writer.Write(item.HasAudioLayer);
            }

            for (var index = 0; index < itemsCount; index++)
            {
                GuiLanguageMapItem item = Items[index];
                var thisPosition = (uint)(writer.BaseStream.Position);
                writer.WriteString(item.DisplayName);
                long currentPosition = writer.BaseStream.Position;
                writer.BaseStream.Position = offsets[index];
                var offset = (uint)(thisPosition - offsets[index]);
                writer.Write(offset);
                writer.BaseStream.Position = currentPosition;
            }
        }
    }
}
