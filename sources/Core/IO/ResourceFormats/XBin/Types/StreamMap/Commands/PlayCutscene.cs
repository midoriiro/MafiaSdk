using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class PlayCutscene: ICommand
    {
        public uint Magic { get; } = 0x90ACE5D5;
        public int Size { get; } = 4;
        
        public string CutsceneName { get; init; } = null!;

        private PlayCutscene()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            string cutsceneName = reader.ReadStringPointerWithOffset();

            return new PlayCutscene()
            {
                CutsceneName = cutsceneName
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushStringPointer(CutsceneName);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(CutsceneName);
        }
    }
}
