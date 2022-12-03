using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands.Vehicle
{
    public class LoadVehicle : ICommand
    {
        public uint Magic { get; } = 0xA1C05A78;

        public int Size
        {
            get
            {
                var totalSize = 32;
                totalSize += (Instances.Count * VehicleInstance.GetSize());
                return totalSize;
            }
        }

        public List<VehicleInstance> Instances { get; init; } = null!;
        public ESlotType SlotType { get; init; }
        public string SdsName { get; init; } = null!;
        public string QuotaId { get; init; } = null!;
        public uint Guid { get; init; }
        public uint SlotId { get; init; }

        private LoadVehicle()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            reader.ReadUInt32(); // offset
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            // TODO test if count is equal zero and throw error
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }

            var slotType = (ESlotType)reader.ReadUInt32();
            string sdsName = reader.ReadStringPointerWithOffset();
            string quotaId = reader.ReadStringPointerWithOffset();
            uint guid = reader.ReadUInt32();
            uint slotId = reader.ReadUInt32();

            var instances = new VehicleInstance[count0];
            
            for(var i = 0; i < instances.Length; i++)
            {
                instances[i] = VehicleInstance.ReadFromFile(reader);
            }

            return new LoadVehicle()
            {
                Instances = instances.ToList(),
                SlotType = slotType,
                SdsName = sdsName,
                QuotaId = quotaId,
                Guid = guid,
                SlotId = slotId
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int instancesCount = Instances.Count;

            writer.PushObjectPointer("VehicleOffset");
            writer.Write(instancesCount); 
            writer.Write(instancesCount);
            writer.Write((uint)SlotType);
            writer.PushStringPointer(SdsName);
            writer.PushStringPointer(QuotaId);
            writer.Write(Guid);
            writer.Write(SlotId);

            writer.FixUpObjectPointer("VehicleOffset");

            foreach (VehicleInstance instance in Instances)
            {
                instance.WriteToFile(writer);
            }
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(SdsName);
            writer.FixUpStringPointer(QuotaId);

            foreach (VehicleInstance instance in Instances)
            {
                instance.FixUpStringPointer(writer);
            }
        }
    }
}
