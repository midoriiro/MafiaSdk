using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class UnlockSds : ICommand
    {
        public uint Magic { get; } = 0xA033FEEB;
        public int Size { get; } = 4;
        
        public string SdsName { get; init; } = null!;

        private UnlockSds()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            string sdsName = reader.ReadStringPointerWithOffset();

            return new UnlockSds()
            {
                SdsName = sdsName
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushStringPointer(SdsName);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(SdsName);
        }
    }
}
