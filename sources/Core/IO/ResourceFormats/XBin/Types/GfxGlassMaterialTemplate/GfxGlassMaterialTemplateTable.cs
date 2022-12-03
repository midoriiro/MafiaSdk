using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.GfxGlassMaterialTemplate
{
    public class GfxGlassMaterialTemplateTable : ITable
    {
        public List<GfxGlassMatTemplateItem> Items { get; init; } = null!;

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
            
            var items = new GfxGlassMatTemplateItem[count0];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                uint originalTemplatePart0 = reader.ReadUInt32();
                uint originalTemplatePart1 = reader.ReadUInt32();
                uint damagedTemplatePart0 = reader.ReadUInt32();
                uint damagedTemplatePart1 = reader.ReadUInt32();
                int type = reader.ReadInt32();
                uint glassBreakType = reader.ReadUInt32();
                string description = reader.ReadStringBuffer(32);
                
                items[i] = new GfxGlassMatTemplateItem()
                {
                    Id = id,
                    OriginalTemplatePart0 = originalTemplatePart0,
                    OriginalTemplatePart1 = originalTemplatePart1,
                    DamagedTemplatePart0 = damagedTemplatePart0,
                    DamagedTemplatePart1 = damagedTemplatePart1,
                    Type = type,
                    GlassBreakType = glassBreakType,
                    Description = description
                };
            }

            return new GfxGlassMaterialTemplateTable()
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
                GfxGlassMatTemplateItem item = Items[i];
                writer.Write(item.Id);
                writer.Write(item.OriginalTemplatePart0);
                writer.Write(item.OriginalTemplatePart1);
                writer.Write(item.DamagedTemplatePart0);
                writer.Write(item.DamagedTemplatePart1);
                writer.Write(item.Type);
                writer.Write(item.GlassBreakType);
                writer.WriteStringBuffer(32, item.Description);
            }
        }
    }
}
