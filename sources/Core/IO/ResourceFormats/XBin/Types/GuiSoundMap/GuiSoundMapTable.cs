using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiSoundMap
{
    public class GuiSoundMapTable : ITable
    {
        public List<GuiSoundMapItem> Items { get; init; } = null!;

        private GuiSoundMapTable()
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
            
            var items = new GuiSoundMapItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                int id = i;
                var soundEvent = (ESoundEvent)reader.ReadUInt32();
                uint wwiseEvent = reader.ReadUInt32();
                
                items[i] = new GuiSoundMapItem()
                {
                    Id = id,
                    SoundEvent = soundEvent,
                    WwiseEvent = wwiseEvent
                };
            }

            return new GuiSoundMapTable()
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
                GuiSoundMapItem item = Items[i];
                writer.Write((uint)item.SoundEvent);
                writer.Write(item.WwiseEvent);
            }
        }
    }
}
