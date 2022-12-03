using Core.IO.ResourceFormats.XBin.Types.FlashInputMap;
using Core.IO.ResourceFormats.XBin.Types.GuiFontMap;
using Core.IO.ResourceFormats.XBin.Types.GuiInputMap;
using Core.IO.ResourceFormats.XBin.Types.GuiLanguageMap;
using Core.IO.ResourceFormats.XBin.Types.GuiSoundMap;

namespace Core.IO.ResourceFormats.XBin.Types.Containers
{
    public class GuiContainer : ITable
    {
        public GuiInputMapTable GuiInputMap { get; init; } = null!;
        public FlashInputMapTable FlashInputMap { get; init; } = null!;
        public GuiFontMapTable GuiFontMap { get; init; } = null!;
        public GuiSoundMapTable GuiSoundMap { get; init; } = null!;
        public GuiLanguageMapTable GuiLanguageMap { get; init; } = null!;

        private GuiContainer()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint guiInputMapPointer = reader.ReadUInt32();
            uint flashInputMapPointer = reader.ReadUInt32();
            uint guiFontMapPointer = reader.ReadUInt32();
            uint guiSoundMapPointer = reader.ReadUInt32();
            uint guiLanguageMapPointer = reader.ReadUInt32();

            uint guiInputMapValue = reader.ReadUInt32();
            var guiInputMap = (GuiInputMapTable)GuiInputMapTable.ReadFromFile(reader);

            uint flashInputMapValue = reader.ReadUInt32();
            var flashInputMap = (FlashInputMapTable)FlashInputMapTable.ReadFromFile(reader);

            uint guiFontMapValue = reader.ReadUInt32();
            var guiFontMap = (GuiFontMapTable)GuiFontMapTable.ReadFromFile(reader);

            uint guiSoundMapValue = reader.ReadUInt32();
            var guiSoundMap = (GuiSoundMapTable)GuiSoundMapTable.ReadFromFile(reader);

            uint guiLanguageMapValue = reader.ReadUInt32();
            var guiLanguageMap = (GuiLanguageMapTable)GuiLanguageMapTable.ReadFromFile(reader);

            return new GuiContainer()
            {
                GuiInputMap = guiInputMap,
                FlashInputMap = flashInputMap,
                GuiFontMap = guiFontMap,
                GuiSoundMap = guiSoundMap,
                GuiLanguageMap = guiLanguageMap
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushObjectPointer("GuiInputMapPointer");
            writer.PushObjectPointer("FlashInputMapPointer");
            writer.PushObjectPointer("GuiFontMapPointer");
            writer.PushObjectPointer("GuiSoundMapPointer");
            writer.PushObjectPointer("GuiLanguageMapPointer");

            // Write GuiInputMap table
            writer.FixUpObjectPointer("GuiInputMapPointer");
            writer.Write(0xC);
            GuiInputMap.WriteToFile(writer);

            // Write FlashInputMap table
            writer.FixUpObjectPointer("FlashInputMapPointer");
            writer.Write(0xC);
            FlashInputMap.WriteToFile(writer);

            // Write GuiFontMap table
            writer.FixUpObjectPointer("GuiFontMapPointer");
            writer.Write(0xC);
            GuiFontMap.WriteToFile(writer);

            // Write GuiSoundMap table
            writer.FixUpObjectPointer("GuiSoundMapPointer");
            writer.Write(0xC);
            GuiSoundMap.WriteToFile(writer);

            // Write GuiLanguageMap table
            writer.FixUpObjectPointer("GuiLanguageMapPointer");
            writer.Write(0xC);
            GuiLanguageMap.WriteToFile(writer);
        }
    }
}
