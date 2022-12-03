using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.Vehicles.Mafia3
{
    public class VehicleTableItem : IVehicleTableItem
    {
        public int Unk0 { get; init; }
        public int Id { get; init; }
        public ETrafficCommonFlags CommonFlags { get; init; } //E_TrafficCommonFlags
        public ETrafficVehicleFlags VehicleFlags { get; init; } //E_TrafficVehicleFlags
        public ETrafficVehicleLookFlags VehicleLookFlags { get; init; } //E_TrafficVehicleLookFlags
        public EVehiclesTableFunctionFlags VehicleFunctionFlags { get; init; } //E_VehiclesTableFunctionFlags
        public string ModelName { get; init; } = null!;
        public string SoundVehicleSwitch { get; init; } = null!;
        public ERadioStation RadioRandom { get; init; } //E_RadioStation
        public float RadioSoundQuality { get; init; }
        public int TexId { get; init; }
        public ulong TexHash { get; init; } //maybe
        public string BrandNameUi { get; init; } = null!;
        public string ModelNameUi { get; init; } = null!;
        public string LogoNameUi { get; init; } = null!;
        public int StealCoefficient { get; init; }
        public int Price { get; init; }
        public float MaxDirt { get; init; }
        public float MaxRust { get; init; }
        public EVehicleRaceClass RaceClass { get; init; } //E_VehicleRaceClass
        public float TrunkDockOffsetX { get; init; }
        public float TrunkDockOffsetY { get; init; }

        private VehicleTableItem()
        {
        }

        public static IVehicleTableItem ReadEntry(BinaryReader reader)
        {
            int unk0 = reader.ReadInt32();
            int id = reader.ReadInt32();
            var commonFlags = (ETrafficCommonFlags)reader.ReadInt32();
            var vehicleFlags = (ETrafficVehicleFlags)reader.ReadInt32();
            var vehicleLookFlags = (ETrafficVehicleLookFlags)reader.ReadInt32();
            var vehicleFunctionFlags = (EVehiclesTableFunctionFlags)reader.ReadInt32();
            string modelName = reader.ReadStringBuffer(32);
            string soundVehicleSwitch = reader.ReadStringBuffer(32);
            var radioRandom = (ERadioStation)reader.ReadInt32();
            float radioSoundQuality = reader.ReadSingle();
            int texId = reader.ReadInt32();
            ulong texHash = reader.ReadUInt64();
            string brandNameUi = reader.ReadStringBuffer(32);
            string modelNameUi = reader.ReadStringBuffer(32);
            string logoNameUi = reader.ReadStringBuffer(32);
            int stealCoefficient = reader.ReadInt32();
            int price = reader.ReadInt32();
            float maxDirt = reader.ReadSingle();
            float maxRust = reader.ReadSingle();
            var raceClass = (EVehicleRaceClass)reader.ReadInt32();
            float trunkDockOffsetX = reader.ReadSingle();
            float trunkDockOffsetY = reader.ReadSingle();

            return new VehicleTableItem()
            {
                Unk0 = unk0,
                Id = id,
                CommonFlags = commonFlags,
                VehicleFlags = vehicleFlags,
                VehicleLookFlags = vehicleLookFlags,
                VehicleFunctionFlags = vehicleFunctionFlags,
                ModelName = modelName,
                SoundVehicleSwitch = soundVehicleSwitch,
                RadioRandom = radioRandom,
                RadioSoundQuality = radioSoundQuality,
                TexId = texId,
                TexHash = texHash,
                BrandNameUi = brandNameUi,
                ModelNameUi = modelNameUi,
                LogoNameUi = logoNameUi,
                StealCoefficient = stealCoefficient,
                Price = price,
                MaxDirt = maxDirt,
                MaxRust = maxRust,
                RaceClass = raceClass,
                TrunkDockOffsetX = trunkDockOffsetX,
                TrunkDockOffsetY = trunkDockOffsetY
            };
        }

        public void WriteEntry(XBinWriter writer)
        {
            writer.Write(Unk0);
            writer.Write(Id);
            writer.Write((int)CommonFlags);
            writer.Write((int)VehicleFlags);
            writer.Write((int)VehicleLookFlags);
            writer.Write((int)VehicleFunctionFlags);
            writer.WriteStringBuffer(32, ModelName);
            writer.WriteStringBuffer(32, SoundVehicleSwitch);
            writer.Write((int)RadioRandom);
            writer.Write(RadioSoundQuality);
            writer.Write(TexId);
            writer.Write(TexHash);
            writer.WriteStringBuffer(32, BrandNameUi);
            writer.WriteStringBuffer(32, ModelNameUi);
            writer.WriteStringBuffer(32, LogoNameUi);
            writer.Write(StealCoefficient);
            writer.Write(Price);
            writer.Write(MaxDirt);
            writer.Write(MaxRust);
            writer.Write((int)RaceClass);
            writer.Write(TrunkDockOffsetX);
            writer.Write(TrunkDockOffsetY);
        }

        public override string ToString()
        {
            return $"{Id} {ModelName}";
        }
    }
}
