using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.FlashInputMap
{
    public class FlashInputMapTable : ITable
    {
        public List<FlashInputMapItem> Items { get; init; } = null!;

        private FlashInputMapTable()
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
            
            var items = new FlashInputMapItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                int id = i;
                var keyModifiers = (EKeyModifiers)reader.ReadUInt32();
                var menuAction = (EMenuAction)reader.ReadUInt32();
                var flashControlType = (EFlashControlType)reader.ReadUInt32();
                var control = (EInputControlUnified)reader.ReadUInt32();
                var axisMode = (EAxisMode)reader.ReadUInt32();
                var deviceIndex = (EFlashDeviceIndex)reader.ReadUInt32();
                
                items[i] = new FlashInputMapItem()
                {
                    Id = id,
                    KeyModifiers = keyModifiers,
                    MenuAction = menuAction,
                    FlashControlType = flashControlType,
                    Control = control,
                    AxisMode = axisMode,
                    DeviceIndex = deviceIndex
                };
            }

            return new FlashInputMapTable()
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
                FlashInputMapItem item = Items[i];
                writer.Write((uint)item.KeyModifiers);
                writer.Write((uint)item.MenuAction);
                writer.Write((uint)item.FlashControlType);
                writer.Write((uint)item.Control);
                writer.Write((uint)item.AxisMode);
                writer.Write((uint)item.DeviceIndex);
            }
        }
    }
}
