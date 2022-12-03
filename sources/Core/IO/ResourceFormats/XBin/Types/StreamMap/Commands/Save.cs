using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class Save : ICommand
    {
        public uint Magic { get; } = 0xEA186B6;
        public int Size { get; } = 68;

        public ESaveType SaveType { get; init; }
        public string SaveId { get; init; } = null!;

        private Save()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            var saveType = (ESaveType)reader.ReadUInt32();
            string saveId = reader.ReadStringBuffer(64);

            return new Save()
            {
                SaveType = saveType,
                SaveId = saveId
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write((uint)SaveType);
            writer.WriteStringBuffer(64, SaveId);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
