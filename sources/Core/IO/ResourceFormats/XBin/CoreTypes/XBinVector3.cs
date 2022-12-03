namespace Core.IO.ResourceFormats.XBin.CoreTypes
{
    public class XBinVector3
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Z { get; init; }

        private XBinVector3()
        {
        }

        public static XBinVector3 ReadFromFile(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();

            return new XBinVector3()
            {
                X = x,
                Y = y,
                Z = z
            };
        }

        public void WriteToFile(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y} Z:{Z}";
        }
    }
}
