using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class PlayVideo : ICommand
    {
        public uint Magic { get; } = 0x687DAD8B;
        public int Size { get; } = 4;
        
        public string VideoName { get; init; } = null!;

        private PlayVideo()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            string videoName = reader.ReadStringPointerWithOffset();

            return new PlayVideo()
            {
                VideoName = videoName
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushStringPointer(VideoName);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(VideoName);
        }
    }
}
