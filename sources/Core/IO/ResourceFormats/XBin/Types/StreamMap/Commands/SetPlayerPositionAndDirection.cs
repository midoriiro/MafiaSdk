using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class SetPlayerPositionAndDirection : ICommand
    {
        public uint Magic { get; } = 0x72386E2B;
        public int Size { get; } = 24;
        
        public XBinVector3 Position { get; init; } = null!;
        public XBinVector3 Direction { get; init; } = null!;

        private SetPlayerPositionAndDirection()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            XBinVector3 position = XBinVector3.ReadFromFile(reader);
            XBinVector3 direction = XBinVector3.ReadFromFile(reader);

            return new SetPlayerPositionAndDirection()
            {
                Position = position,
                Direction = direction
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            Position.WriteToFile(writer);
            Direction.WriteToFile(writer);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            // No string pointer to fix up
        }
    }
}
