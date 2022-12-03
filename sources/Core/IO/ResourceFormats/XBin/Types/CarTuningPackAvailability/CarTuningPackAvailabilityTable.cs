namespace Core.IO.ResourceFormats.XBin.Types.CarTuningPackAvailability
{
    public class CarTuningPackAvailabilityTable : ITable
    {
        public uint Unk0 { get; init; }
        public uint Unk1 { get; init; }
        public List<CarTuningPackAvailabilityItem> Items { get; init; } = null!;

        private CarTuningPackAvailabilityTable()
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
            
            uint unk1 = reader.ReadUInt32();
            var items = new CarTuningPackAvailabilityItem[count0];

            for (var i = 0; i < count1; i++)
            {
                items[i] = CarTuningPackAvailabilityItem.ReadFromFile(reader);
            }

            for (var i = 0; i < count1; i++)
            {
                CarTuningPackAvailabilityItem item = items[i];
                for (var z = 0; z < item.TuningItems.Length; z++)
                {
                    item.TuningItems[z] = reader.ReadInt32();
                }
            }

            return new CarTuningPackAvailabilityTable()
            {
                Unk0 = unk0,
                Unk1 = unk1,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);
            writer.Write(Unk1);
            
            var offsets = new long[itemsCount];

            for (var index = 0; index < Items.Count; index++)
            {
                CarTuningPackAvailabilityItem item = Items[index];
                writer.Write(item.Id);
                offsets[index] = writer.BaseStream.Position;
                writer.Write(0); //placeholder
                writer.Write(item.TuningItems.Length);
                writer.Write(item.TuningItems.Length);
                writer.Write(item.VehicleId);
                writer.Write(item.Zero);
                item.PackageName.WriteToFile(writer);
            }

            for (var index = 0; index < itemsCount; index++)
            {
                CarTuningPackAvailabilityItem item = Items[index];
                
                var thisPosition = (uint)(writer.BaseStream.Position);
                
                foreach (int tuningItem in item.TuningItems)
                {
                    writer.Write(tuningItem);
                }

                long currentPosition = writer.BaseStream.Position;
                writer.BaseStream.Position = offsets[index];
                var offset = (uint)(thisPosition - offsets[index]);
                writer.Write(offset);
                writer.BaseStream.Position = currentPosition;
            }
        }
    }
}
