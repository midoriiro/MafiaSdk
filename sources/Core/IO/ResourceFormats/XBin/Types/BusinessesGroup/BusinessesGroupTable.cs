namespace Core.IO.ResourceFormats.XBin.Types.BusinessesGroup
{
    public class BusinessesGroupTable : ITable
    {
        private BusinessesGroupTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            int unk0 = reader.ReadInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();

            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }

            return new BusinessesGroupTable()
            {
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
        }
    }
}
