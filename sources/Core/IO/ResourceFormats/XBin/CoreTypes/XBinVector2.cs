namespace Core.IO.ResourceFormats.XBin.CoreTypes
{
    public class XBinVector2
    {
        public float X { get; init; }
        public float Y { get; init; }

        private XBinVector2()
        {
        }

        public static XBinVector2 ReadFromFile(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();

            return new XBinVector2()
            {
                X = x,
                Y = y
            };
        }

        public void WriteToFile(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }
    }
}
