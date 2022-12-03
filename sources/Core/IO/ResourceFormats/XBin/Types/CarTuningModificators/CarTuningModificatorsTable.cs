namespace Core.IO.ResourceFormats.XBin.Types.CarTuningModificators
{
    public class CarTuningModificatorsTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarTuningModificatorsItem> Items { get; init; } = null!;

        private CarTuningModificatorsTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new CarTuningModificatorsItem[count0];

            for (var i = 0; i < count1; i++)
            {
                int id = reader.ReadInt32();
                int carId = reader.ReadInt32();
                int itemId = reader.ReadInt32();
                int memberId = reader.ReadInt32();
                int value = reader.ReadInt32();

                items[i] = new CarTuningModificatorsItem()
                {
                    Id = id,
                    CarId = carId,
                    ItemId = itemId,
                    MemberId = memberId,
                    Value = value
                };
            }

            return new CarTuningModificatorsTable()
            {
                Unk0 = unk0,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            foreach(CarTuningModificatorsItem modificator in Items)
            {
                writer.Write(modificator.Id);
                writer.Write(modificator.CarId);
                writer.Write(modificator.ItemId);
                writer.Write(modificator.MemberId);
                writer.Write(modificator.Value);
            }
        }
    }
}
