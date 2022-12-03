using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.String
{
    public class StringTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<LocalisableString> Items { get; init; } = null!;

        private StringTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            // XBin files store the count twice.
            uint unk0 = reader.ReadUInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();

            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }

            var items = new LocalisableString[count0];

            for (var i = 0; i < count0; i++)
            {
                uint localisableStringUnk0 = reader.ReadUInt32();
                XBinHash64Name stringId = XBinHash64Name.ReadFromFile(reader);
                string content = reader.ReadStringPointerWithOffset();
                
                items[i] = new LocalisableString()
                {
                    Unk0 = localisableStringUnk0,
                    StringId = stringId,
                    Content = content
                };
            }

            return new StringTable()
            {
                Unk0 = unk0,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            // Order entries.
            var orderedItems = Items
                .OrderBy(entry => entry.StringId.Hash)
                .ToArray();
            int itemsCount = orderedItems.Length;
            
            writer.Write(Unk0);
            writer.Write(itemsCount); // count0
            writer.Write(itemsCount); // count1

            // Idea is to write all entries and come back to replace Ptr(offset).
            var positions = new long[itemsCount];
            
            for(var i = 0; i < itemsCount; i++)
            {
                LocalisableString entry = orderedItems[i];
                
                writer.Write(entry.Unk0);
                entry.StringId.WriteToFile(writer);

                positions[i] = writer.BaseStream.Position;
                
                writer.Write(-1);
            }
            
            // Seems to be padding. Concerning..
            writer.Write(0);

            for(var i = 0; i < itemsCount; i++)
            {
                LocalisableString entry = orderedItems[i];

                // We get the position
                var positionBeforeWritingContent = (uint)writer.BaseStream.Position;
                writer.WriteString(entry.Content);
                long currentPosition = writer.BaseStream.Position;

                // Correct the offset and write to the file
                writer.BaseStream.Position = positions[i];
                var offset = (uint)(positionBeforeWritingContent - positions[i]);
                writer.Write(offset);
                writer.BaseStream.Position = currentPosition;
            }
        }
    }
}
