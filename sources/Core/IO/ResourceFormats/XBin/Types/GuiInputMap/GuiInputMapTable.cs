using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiInputMap
{
    public class GuiInputMapTable : ITable
    {
        public List<GuiInputMapItem> Items { get; init; } = null!;


        private GuiInputMapTable()
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
            
            var items = new GuiInputMapItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                int id = i;
                var controlMode = (EInputControlFlags)reader.ReadUInt32();
                var keyModifiers = (EKeyModifiers)reader.ReadUInt32();
                var deviceType = (EInputDeviceType)reader.ReadUInt32();
                var control = (EInputControlUnified)reader.ReadUInt32();
                var controlPriority = (EControllerType)reader.ReadUInt32();
                var menuAction = (EMenuAction)reader.ReadUInt32();
                
                items[i] = new GuiInputMapItem()
                {
                    Id = id,
                    ControlMode = controlMode,
                    KeyModifiers = keyModifiers,
                    DeviceType = deviceType,
                    Control = control,
                    ControlPriority = controlPriority,
                    MenuAction = menuAction
                };
            }

            return new GuiInputMapTable()
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
                GuiInputMapItem item = Items[i];
                writer.Write((uint)item.ControlMode);
                writer.Write((uint)item.KeyModifiers);
                writer.Write((uint)item.DeviceType);
                writer.Write((uint)item.Control);
                writer.Write((uint)item.ControlPriority);
                writer.Write((uint)item.MenuAction);
            }
        }
    }
}
