using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Types.Vehicles.Mafia3;

namespace Core.IO.ResourceFormats.XBin.Types.CarTrafficTuning
{
    public class CarTrafficTuningTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarTrafficTuningItem> Items { get; init; } = null!;

        private CarTrafficTuningTable()
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
            
            reader.ReadUInt32(); // padding
            var items = new CarTrafficTuningItem[count0];

            for (var i = 0; i < count1; i++)
            {
                int id = reader.ReadInt32();
                int collectionOffset = reader.ReadInt32();
                int collectionCount1 = reader.ReadInt32();
                int collectionCount2 = reader.ReadInt32();
                int vehicleId = reader.ReadInt32();
                var vehicleFlags = (ETrafficVehicleFlags)reader.ReadUInt32();
                var vehicleLookFlags = (ETrafficVehicleLookFlags)reader.ReadUInt32();
                float weight = reader.ReadSingle();
                var name = XBinHash64Name.ReadFromFile(reader);

                items[i] = new CarTrafficTuningItem()
                {
                    Id = id,
                    CollectionOffset = collectionOffset,
                    CollectionCount1 = collectionCount1,
                    CollectionCount2 = collectionCount2,
                    VehicleId = vehicleId,
                    VehicleFlags = vehicleFlags,
                    VehicleLookFlags = vehicleLookFlags,
                    Weight = weight,
                    Name = name
                };
            }

            for (var i = 0; i < count1; i++)
            {
                CarTrafficTuningItem item = items[i];
                
                item.TuningItems = new int[item.CollectionCount1];
                
                for (var z = 0; z < item.CollectionCount1; z++)
                {
                    item.TuningItems[z] = reader.ReadInt32();
                }
            }

            return new CarTrafficTuningTable()
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
            writer.Write(0); // padding
            
            var offsets = new long[itemsCount];

            for (var index = 0; index < itemsCount; index++)
            {
                CarTrafficTuningItem item = Items[index];
                item.CollectionCount1 = item.TuningItems.Length;
                item.CollectionCount2 = item.TuningItems.Length;
                writer.Write(item.Id);
                offsets[index] = writer.BaseStream.Position;
                writer.Write(0xDEADBEEF); //placeholder
                writer.Write(item.TuningItems.Length);
                writer.Write(item.TuningItems.Length);
                writer.Write(item.VehicleId);
                writer.Write((int)item.VehicleFlags);
                writer.Write((int)item.VehicleLookFlags);
                writer.Write(item.Weight);
                item.Name.WriteToFile(writer);
            }

            for (var index = 0; index < itemsCount; index++)
            {
                CarTrafficTuningItem item = Items[index];
                var thisPosition = (uint)(writer.BaseStream.Position);

                for (var z = 0; z < item.CollectionCount1; z++)
                {
                    writer.Write(item.TuningItems[z]);
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
