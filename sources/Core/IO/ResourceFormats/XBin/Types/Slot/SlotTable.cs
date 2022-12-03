using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.Slot
{
    public class SlotTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<SlotItem> Items { get; init; } = null!;

        private SlotTable()
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
            
            var items = new SlotItem[count0];

            for (var i = 0; i < count1; i++)
            {
                int typeId = reader.ReadInt32();
                var slotType = (ESlotType)reader.ReadInt32();
                int baseNameOffset = reader.ReadInt32();
                uint ramWindows = reader.ReadUInt32();
                uint vramWindows = reader.ReadUInt32();
                uint ramXbox360 = reader.ReadUInt32();
                uint vramXbox360 = reader.ReadUInt32();
                uint ramPs3Devkit = reader.ReadUInt32();
                uint vramPs3Devkit = reader.ReadUInt32();
                uint ramPs3Testkit = reader.ReadUInt32();
                uint vramPs3Testkit = reader.ReadUInt32();

                items[i] = new SlotItem()
                {
                    TypeId = typeId,
                    SlotType = slotType,
                    BaseNameOffset = baseNameOffset,
                    RamWindows = ramWindows,
                    VramWindows = vramWindows,
                    RamXbox360 = ramXbox360,
                    VramXbox360 = vramXbox360,
                    RamPs3Devkit = ramPs3Devkit,
                    VramPs3Devkit = vramPs3Devkit,
                    RamPs3Testkit = ramPs3Testkit,
                    VramPs3Testkit = vramPs3Testkit
                };
            }

            for (var i = 0; i < count1; i++)
            {
                SlotItem item = items[i];
                item.BaseName = reader.ReadString().TrimEnd('\0');
                items[i] = item;
            }

            return new SlotTable()
            {
                Unk0 = unk0,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            var i = 0;
            var offsets = new long[itemsCount];
            
            foreach (SlotItem slot in Items)
            {
                SlotItem item = Items[i];
                writer.Write(item.TypeId);
                writer.Write((int)slot.SlotType);
                offsets[i] = writer.BaseStream.Position;
                writer.Write(0xDEADBEEF); // temporary
                writer.Write(item.RamWindows);
                writer.Write(item.VramWindows);
                writer.Write(item.RamXbox360);
                writer.Write(item.VramXbox360);
                writer.Write(item.RamPs3Devkit);
                writer.Write(item.VramPs3Devkit);
                writer.Write(item.RamPs3Testkit);
                writer.Write(item.VramPs3Testkit);
                i++;
            }

            for (var j = 0; j < itemsCount; j++)
            {
                SlotItem item = Items[j];
                var thisPosition = (uint)(writer.BaseStream.Position);
                writer.WriteString(item.BaseName);

                long currentPosition = writer.BaseStream.Position;
                writer.BaseStream.Position = offsets[j];
                var offset = (uint)(thisPosition - offsets[j]);
                writer.Write(offset);
                writer.BaseStream.Position = currentPosition;
            }
        }
    }
}
