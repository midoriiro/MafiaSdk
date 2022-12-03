using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.CarTuningItem
{
    public class CarTuningItemTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarTuningItemTableItem> Items { get; init; } = null!;

        private CarTuningItemTable()
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
            
            var items = new CarTuningItemTableItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                uint slotId = reader.ReadUInt32();
                string description = reader.ReadStringBuffer(32);
                var flags = (ECarTuningItemFlags)reader.ReadUInt32();
                
                string tyreFront = reader.ReadStringBuffer(8);
                string tyreRear = reader.ReadStringBuffer(8);
                 
                float engineTorqueMinRot = reader.ReadSingle();
                float engineTorque = reader.ReadSingle();
                float engineTorqueMaxRot = reader.ReadSingle();
                float enginePowerAndTorqueRotations = reader.ReadSingle();
                 
                float engineMaxRotations = reader.ReadSingle();
                float engineBrakeTorque = reader.ReadSingle();
                float engineInertia = reader.ReadSingle();
                float engineEfficiency = reader.ReadSingle();
                
                float engineTurboMinimalRotations = reader.ReadSingle();
                float engineTurboOptimalRotations = reader.ReadSingle();
                float engineTurboTurnOnTime = reader.ReadSingle();
                float engineTurboTorqueIncrease = reader.ReadSingle();
                
                int gearbox = reader.ReadInt32();
                float finalGear = reader.ReadSingle();
                float viscousClutch = reader.ReadSingle();
                
                float frontSpringLength = reader.ReadSingle();
                float frontSpringStiffness = reader.ReadSingle();
                float frontDamperStiffness = reader.ReadSingle();
                float frontSwayBar = reader.ReadSingle();
                float frontTyrePressure = reader.ReadSingle();
                
                float rearSpringLength = reader.ReadSingle();
                float rearSpringStiffness = reader.ReadSingle();
                float rearDamperStiffness = reader.ReadSingle();
                float rearSwayBar = reader.ReadSingle();
                float rearTyrePressure = reader.ReadSingle();
                
                float breakTorque = reader.ReadSingle();
                float breakEfficiency = reader.ReadSingle();
                float breakReaction = reader.ReadSingle();
                
                float frontSpoilerCoefficient = reader.ReadSingle();
                float rearSpoilerCoefficient = reader.ReadSingle();
                float aerodynamic = reader.ReadSingle();
                
                uint vehicleBodyMaterialId = reader.ReadUInt32();
                uint vehicleWindowMaterialId = reader.ReadUInt32();
                
                float vehicleMass = reader.ReadSingle();
                float engineResistance = reader.ReadSingle();
                float vehicleBodyBoneStiffness = reader.ReadSingle();
                float crashSpeedChange = reader.ReadSingle();
                float carCrewCrashSpeedChange = reader.ReadSingle();

                items[i] = new CarTuningItemTableItem()
                {
                    Id = id,
                    SlotId = slotId,
                    Description = description,
                    Flags = flags,
                    
                    TyreFront = tyreFront,
                    TyreRear = tyreRear,
                    
                    EngineTorqueMinimalRotations = engineTorqueMinRot,
                    EngineTorque = engineTorque,
                    EngineTorqueMaximalRotations = engineTorqueMaxRot,
                    EnginePowerAndTorqueRotations = enginePowerAndTorqueRotations,
                    
                    EngineMaxRotations = engineMaxRotations,
                    EngineBrakeTorque = engineBrakeTorque,
                    EngineInertia = engineInertia,
                    EngineEfficiency = engineEfficiency,
                    
                    EngineTurboMinimalRotations = engineTurboMinimalRotations,
                    EngineTurboOptimalRotations = engineTurboOptimalRotations,
                    EngineTurboTurnOnTime = engineTurboTurnOnTime,
                    EngineTurboTorqueIncrease = engineTurboTorqueIncrease,
                    
                    Gearbox = gearbox,
                    FinalGear = finalGear,
                    ViscousClutch = viscousClutch,
                    
                    FrontSpringLength = frontSpringLength,
                    FrontSpringStiffness = frontSpringStiffness,
                    FrontDamperStiffness = frontDamperStiffness,
                    FrontSwayBar = frontSwayBar,
                    FrontTyrePressure = frontTyrePressure,
                    
                    RearSpringLength = rearSpringLength,
                    RearSpringStiffness = rearSpringStiffness,
                    RearDamperStiffness = rearDamperStiffness,
                    RearSwayBar = rearSwayBar,
                    RearTyrePressure = rearTyrePressure,
                    
                    BreakTorque = breakTorque,
                    BreakEfficiency = breakEfficiency,
                    BreakReaction = breakReaction,
                    
                    FrontSpoilerCoefficient = frontSpoilerCoefficient,
                    RearSpoilerCoefficient = rearSpoilerCoefficient,
                    Aerodynamic = aerodynamic,
                    
                    VehicleBodyMaterialId = vehicleBodyMaterialId,
                    VehicleWindowMaterialId = vehicleWindowMaterialId,
                    
                    VehicleMass = vehicleMass,
                    EngineResistance = engineResistance,
                    VehicleBodyBoneStiffness = vehicleBodyBoneStiffness ,
                    CrashSpeedChange = crashSpeedChange,
                    CarCrewCrashSpeedChange = carCrewCrashSpeedChange,
                };
            }

            return new CarTuningItemTable()
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

            for (var i = 0; i < itemsCount; i++)
            {
                CarTuningItemTableItem carTuningItem = Items[i];
                writer.Write(carTuningItem.Id);
                writer.Write(carTuningItem.SlotId);
                writer.WriteStringBuffer(32, carTuningItem.Description);
                writer.Write((uint)carTuningItem.Flags);
                
                writer.WriteStringBuffer(8, carTuningItem.TyreFront);
                writer.WriteStringBuffer(8, carTuningItem.TyreRear);
                
                writer.Write(carTuningItem.EngineTorqueMinimalRotations);
                writer.Write(carTuningItem.EngineTorque);
                writer.Write(carTuningItem.EngineTorqueMaximalRotations);
                writer.Write(carTuningItem.EnginePowerAndTorqueRotations);

                writer.Write(carTuningItem.EngineMaxRotations);
                writer.Write(carTuningItem.EngineBrakeTorque);
                writer.Write(carTuningItem.EngineInertia);
                writer.Write(carTuningItem.EngineEfficiency);

                writer.Write(carTuningItem.EngineTurboMinimalRotations);
                writer.Write(carTuningItem.EngineTurboOptimalRotations);
                writer.Write(carTuningItem.EngineTurboTurnOnTime);
                writer.Write(carTuningItem.EngineTurboTorqueIncrease);

                writer.Write(carTuningItem.Gearbox);
                writer.Write(carTuningItem.FinalGear);
                writer.Write(carTuningItem.ViscousClutch);

                writer.Write(carTuningItem.FrontSpringLength);
                writer.Write(carTuningItem.FrontSpringStiffness);
                writer.Write(carTuningItem.FrontDamperStiffness);
                writer.Write(carTuningItem.FrontSwayBar);
                writer.Write(carTuningItem.FrontTyrePressure);

                writer.Write(carTuningItem.RearSpringLength);
                writer.Write(carTuningItem.RearSpringStiffness);
                writer.Write(carTuningItem.RearDamperStiffness);
                writer.Write(carTuningItem.RearSwayBar);
                writer.Write(carTuningItem.RearTyrePressure);

                writer.Write(carTuningItem.BreakTorque);
                writer.Write(carTuningItem.BreakEfficiency);
                writer.Write(carTuningItem.BreakReaction);

                writer.Write(carTuningItem.FrontSpoilerCoefficient);
                writer.Write(carTuningItem.RearSpoilerCoefficient);
                writer.Write(carTuningItem.Aerodynamic);

                writer.Write(carTuningItem.VehicleBodyMaterialId);
                writer.Write(carTuningItem.VehicleWindowMaterialId);

                writer.Write(carTuningItem.VehicleMass);
                writer.Write(carTuningItem.EngineResistance);
                writer.Write(carTuningItem.VehicleBodyBoneStiffness);
                writer.Write(carTuningItem.CrashSpeedChange);
                writer.Write(carTuningItem.CarCrewCrashSpeedChange);
            }
        }
    }
}
