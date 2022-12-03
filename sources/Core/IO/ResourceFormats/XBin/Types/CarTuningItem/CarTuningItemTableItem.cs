using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.CarTuningItem;

public class CarTuningItemTableItem
{
    public uint Id { get; set; }
    public uint SlotId { get; set; }
    public string Description { get; set; } = null!;
    public ECarTuningItemFlags Flags { get; set; }

    public string TyreFront { get; set; } = null!;
    public string TyreRear { get; set; } = null!;

    public float EngineTorqueMinimalRotations { get; set; }
    public float EngineTorque { get; set; }
    public float EngineTorqueMaximalRotations { get; set; }
    public float EnginePowerAndTorqueRotations { get; set; }

    public float EngineMaxRotations { get; set; }
    public float EngineBrakeTorque { get; set; }
    public float EngineInertia { get; set; }
    public float EngineEfficiency { get; set; }

    public float EngineTurboMinimalRotations { get; set; }
    public float EngineTurboOptimalRotations { get; set; }
    public float EngineTurboTurnOnTime { get; set; }
    public float EngineTurboTorqueIncrease { get; set; }

    public int Gearbox { get; set; }
    public float FinalGear { get; set; }
    public float ViscousClutch { get; set; }

    public float FrontSpringLength { get; set; }
    public float FrontSpringStiffness { get; set; }
    public float FrontDamperStiffness { get; set; }
    public float FrontSwayBar { get; set; }
    public float FrontTyrePressure { get; set; }

    public float RearSpringLength { get; set; }
    public float RearSpringStiffness { get; set; }
    public float RearDamperStiffness { get; set; }
    public float RearSwayBar { get; set; }
    public float RearTyrePressure { get; set; }

    public float BreakTorque { get; set; }
    public float BreakEfficiency { get; set; }
    public float BreakReaction { get; set; }

    public float FrontSpoilerCoefficient { get; set; }
    public float RearSpoilerCoefficient { get; set; }
    public float Aerodynamic { get; set; }

    public uint VehicleBodyMaterialId { get; set; }
    public uint VehicleWindowMaterialId { get; set; }

    public float VehicleMass { get; set; }
    public float EngineResistance { get; set; }
    public float VehicleBodyBoneStiffness { get; set; }
    public float CrashSpeedChange { get; set; }
    public float CarCrewCrashSpeedChange { get; set; }

    internal CarTuningItemTableItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} - {Description}";
    }
}