namespace Core.IO.ResourceFormats.XBin.Types.CarGearBoxes;

public class CarGearBoxesItem
{
    public int Id { get; init; }
    public string Description { get; init; } = null!;
    public int Automatic { get; init; }
    public int GearCount { get; init; }
    public int GearReverseCount { get; init; }
    public float GearRatio0 { get; init; }
    public float RotationsGearUp0 { get; init; }
    public float RotationsGearDown0 { get; init; }
    public float GearRatio1 { get; init; }
    public float RotationsGearUp1 { get; init; }
    public float RotationsGearDown1 { get; init; }
    public float GearRatio2 { get; init; }
    public float RotationsGearUp2 { get; init; }
    public float RotationsGearDown2 { get; init; }
    public float GearRatio3 { get; init; }
    public float RotationsGearUp3 { get; init; }
    public float RotationsGearDown3 { get; init; }
    public float GearRatio4 { get; init; }
    public float RotationsGearUp4 { get; init; }
    public float RotationsGearDown4 { get; init; }
    public float GearRatio5 { get; init; }
    public float RotationsGearUp5 { get; init; }
    public float RotationsGearDown5 { get; init; }
    public float GearRatio6 { get; init; }
    public float RotationsGearUp6 { get; init; }
    public float RotationsGearDown6 { get; init; }
    public float MinClutchGlobal { get; init; }
    public float MinClutchAngleCoefficientGlobal { get; init; }
    public float ShiftDelayMin { get; init; }
    public float ShiftDelayMax { get; init; }

    internal CarGearBoxesItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} {Description}";
    }
}