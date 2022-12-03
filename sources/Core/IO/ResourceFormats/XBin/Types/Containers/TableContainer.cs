using Core.IO.ResourceFormats.XBin.Extensions;
using Core.IO.ResourceFormats.XBin.Types.CarColors;
using Core.IO.ResourceFormats.XBin.Types.CarMaterialStuff;
using Core.IO.ResourceFormats.XBin.Types.CarSkidmarks;
using Core.IO.ResourceFormats.XBin.Types.CarTuningItem;
using Core.IO.ResourceFormats.XBin.Types.CarWindowsTint;
using Core.IO.ResourceFormats.XBin.Types.HealthSystem;
using Core.IO.ResourceFormats.XBin.Types.HumanDamageZones;
using Core.IO.ResourceFormats.XBin.Types.HumanMaterials;
using Core.IO.ResourceFormats.XBin.Types.HumanWeaponImpact;
using Core.IO.ResourceFormats.XBin.Types.MaterialsPhysics;
using Core.IO.ResourceFormats.XBin.Types.MaterialsShots;

namespace Core.IO.ResourceFormats.XBin.Types.Containers
{
    public class TableContainer : ITable
    {
        public uint AiWeaponPointer { get; init; } // Not implemented in Toolkit.
        public uint AnimParticlesPointer { get; init; } // Not implemented in game.
        public uint AttackParametersPointer { get; init; } // Not implemented in game.
        public CarColorsTable CarColors { get; init; } = null!;
        public CarWindowTintTable CarWindowTints { get; init; } = null!;
        public uint CarInteriorColorsTableMpPointer { get; init; } // Not implemented in game.
        public uint CarGearboxesTableMpPointer { get; init; } // Not implemented in game.
        public CarMaterialStuffTable CarMaterialStuff { get; init; } = null!;
        public CarSkidmarksTable CarSkidmarks { get; init; } = null!;
        public CarTuningItemTable CarTuningItems { get; init; } = null!;
        public uint CarTuningModificatorsTableMpPointer { get; init; } // Not implemented in game.
        public uint CombinableCharactersTableMpPointer { get; init; } // Not implemented in game.
        public uint CrashObjectTablePointer { get; init; } // Not implemented in game.
        public uint CubeMapsTablePointer { get; init; } // Not implemented in game.
        public uint DamageMultiplierTablePointer { get; init; } // Not implemented in game.
        public uint FamilyAlbumExtrasTablePointer { get; init; } // Not implemented in game.
        public uint FamilyAlbumTablePointer { get; init; } // Not implemented in game.
        public HealthSystemTable HealthSystem { get; init; } = null!;
        public HumanWeaponImpactTable HumanWeaponImpacts { get; init; } = null!;
        public HumanDamageZonesTable HumanDamageZones { get; init; } = null!;
        public HumanMaterialsTable HumanMaterials { get; init; } = null!;
        public MaterialsPhysicsTable MaterialPhysics { get; init; } = null!;
        public MaterialsShotsTable MaterialShots { get; init; } = null!;

        private TableContainer()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            // NB: Only suitable for M3 for now.
            uint aiWeaponPointer = reader.ReadUInt32();
            uint animParticlesPointer = reader.ReadUInt32();
            uint attackParametersPointer = reader.ReadUInt32();

            long currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var carColors = (CarColorsTable)CarColorsTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;

            reader.GotoPointerWithOffset();

            var carWindowTints = (CarWindowTintTable)CarWindowTintTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

            uint carInteriorColorsTableMpPointer = reader.ReadUInt32();
            uint carGearboxesTableMpPointer = reader.ReadUInt32();

            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var carMaterialStuff = (CarMaterialStuffTable)CarMaterialStuffTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var carSkidmarks = (CarSkidmarksTable)CarSkidmarksTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var carTuningItems = (CarTuningItemTable)CarTuningItemTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);

            uint carTuningModificatorsTableMpPointer = reader.ReadUInt32();
            uint combinableCharactersTableMpPointer = reader.ReadUInt32();
            uint crashObjectTablePointer = reader.ReadUInt32();
            uint cubeMapsTablePointer = reader.ReadUInt32();
            uint damageMultiplierTablePointer = reader.ReadUInt32();
            uint familyAlbumExtrasTablePointer = reader.ReadUInt32();
            uint familyAlbumTablePointer = reader.ReadUInt32();

            currentPosition = reader.BaseStream.Position;
            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var healthSystem = (HealthSystemTable)HealthSystemTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var humanWeaponImpacts = (HumanWeaponImpactTable)HumanWeaponImpactTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var humanDamageZones = (HumanDamageZonesTable)HumanDamageZonesTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var humanMaterials = (HumanMaterialsTable)HumanMaterialsTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var materialPhysics = (MaterialsPhysicsTable)MaterialsPhysicsTable.ReadFromFile(reader);

            reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            currentPosition = reader.BaseStream.Position + 4;
            reader.GotoPointerWithOffset();

            var materialShots = (MaterialsShotsTable)MaterialsShotsTable.ReadFromFile(reader);

            // TODO: Everything in this function was always "temporary".
            // Maybe check the other table container files, see if they 
            // are good enough. Otherwise I need to create a new solution

            return new TableContainer()
            {
                AiWeaponPointer = aiWeaponPointer,
                AnimParticlesPointer = animParticlesPointer,
                AttackParametersPointer = attackParametersPointer,
                CarColors = carColors,
                CarWindowTints = carWindowTints,
                CarInteriorColorsTableMpPointer = carInteriorColorsTableMpPointer,
                CarGearboxesTableMpPointer = carGearboxesTableMpPointer,
                CarMaterialStuff = carMaterialStuff,
                CarSkidmarks = carSkidmarks,
                CarTuningItems = carTuningItems,
                CarTuningModificatorsTableMpPointer = carTuningModificatorsTableMpPointer,
                CombinableCharactersTableMpPointer = combinableCharactersTableMpPointer,
                CrashObjectTablePointer = crashObjectTablePointer,
                CubeMapsTablePointer = cubeMapsTablePointer,
                DamageMultiplierTablePointer = damageMultiplierTablePointer,
                FamilyAlbumExtrasTablePointer = familyAlbumExtrasTablePointer,
                FamilyAlbumTablePointer = familyAlbumTablePointer,
                HealthSystem = healthSystem,
                HumanWeaponImpacts = humanWeaponImpacts,
                HumanDamageZones = humanDamageZones,
                HumanMaterials = humanMaterials,
                MaterialPhysics = materialPhysics,
                MaterialShots = materialShots
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
