using Core.IO.ResourceFormats.XBin.Types.DecalGroupPattern;
using Core.IO.ResourceFormats.XBin.Types.DecalPattern;
using Core.IO.ResourceFormats.XBin.Types.GfxGlassBreakType;
using Core.IO.ResourceFormats.XBin.Types.GfxGlassMaterialTemplate;
using Core.IO.ResourceFormats.XBin.Types.MultiDecalPattern;

namespace Core.IO.ResourceFormats.XBin.Types.Containers
{
    public class GfxContainer : ITable
    {
        public DecalGroupPatternTable DecalGroupPattern { get; init; } = null!;
        public DecalPatternTable DecalPattern { get; init; } = null!;
        public GfxGlassBreakTypeTable GfxGlassBreakType { get; init; } = null!;
        public GfxGlassMaterialTemplateTable GfxGlassMaterialTemplate { get; init; } = null!;
        public MultiDecalPatternTable MultiDecalPattern { get; init; } = null!;

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint decalGroupPatternPointer = reader.ReadUInt32();
            uint decalPatternPointer = reader.ReadUInt32();
            uint gfxGlassBreakTypePointer = reader.ReadUInt32();
            uint gfxGlassMatTemplatePointer = reader.ReadUInt32();
            uint multiDecalPatternPointer = reader.ReadUInt32();

            uint decalGroupPatternsValue = reader.ReadUInt32();
            var decalGroupPattern = (DecalGroupPatternTable)DecalGroupPatternTable.ReadFromFile(reader);

            uint decalPatternValue = reader.ReadUInt32();
            var decalPattern = (DecalPatternTable)DecalPatternTable.ReadFromFile(reader);

            uint gfxGlassBreakTypeValue = reader.ReadUInt32();
            var gfxGlassBreakType = (GfxGlassBreakTypeTable)GfxGlassBreakTypeTable.ReadFromFile(reader);

            uint gfxGlassMatTemplateValue = reader.ReadUInt32();
            var gfxGlassMaterialTemplate = (GfxGlassMaterialTemplateTable)GfxGlassMaterialTemplateTable.ReadFromFile(reader);

            uint multiDecalPatternValue = reader.ReadUInt32();
            var multiDecalPattern = (MultiDecalPatternTable)MultiDecalPatternTable.ReadFromFile(reader);

            return new GfxContainer()
            {
                DecalGroupPattern = decalGroupPattern,
                DecalPattern = decalPattern,
                GfxGlassBreakType = gfxGlassBreakType,
                GfxGlassMaterialTemplate = gfxGlassMaterialTemplate,
                MultiDecalPattern = multiDecalPattern
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushObjectPointer("DecalGroupPatternPointer");
            writer.PushObjectPointer("DecalPatternPointer");
            writer.PushObjectPointer("GfxGlassBreakTypePointer");
            writer.PushObjectPointer("GfxGlassMatTemplatePointer");
            writer.PushObjectPointer("MultiDecalPatternPointer");

            // Write DecalGroupPattern table
            writer.FixUpObjectPointer("DecalGroupPatternPointer");
            writer.Write(0xC);
            DecalGroupPattern.WriteToFile(writer);

            // Write DecalPattern table
            writer.FixUpObjectPointer("DecalPatternPointer");
            writer.Write(0x10);
            DecalPattern.WriteToFile(writer);

            // Write GfxGlassBreakTypePtr table
            writer.FixUpObjectPointer("GfxGlassBreakTypePointer");
            writer.Write(0xC);
            GfxGlassBreakType.WriteToFile(writer);

            // Write GfxGlassMatTemplate table
            writer.FixUpObjectPointer("GfxGlassMatTemplatePointer");
            writer.Write(0xC);
            GfxGlassMaterialTemplate.WriteToFile(writer);

            // Write MultiDecalPattern table
            writer.FixUpObjectPointer("MultiDecalPatternPointer");
            writer.Write(0xC);
            MultiDecalPattern.WriteToFile(writer);
        }
    }
}
