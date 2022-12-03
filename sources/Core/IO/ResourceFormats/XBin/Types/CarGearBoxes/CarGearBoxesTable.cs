using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.CarGearBoxes
{
    public class CarGearBoxesTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<CarGearBoxesItem> Items { get; init; } = null!;

        private CarGearBoxesTable()
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
            
            var items = new CarGearBoxesItem[count0];

            for (var i = 0; i < count1; i++)
            {
                int id = reader.ReadInt32();
                string description = reader.ReadStringBuffer(32).TrimEnd('\0');
                int automatic = reader.ReadInt32(); //bool
                int gearCount = reader.ReadInt32();
                int gearReverseCount = reader.ReadInt32();
                float gearRatio0 = reader.ReadSingle();
                float rotationsGearUp0 = reader.ReadSingle();
                float rotationsGearDown0 = reader.ReadSingle();
                float gearRatio1 = reader.ReadSingle();
                float rotationsGearUp1 = reader.ReadSingle();
                float rotationsGearDown1 = reader.ReadSingle();
                float gearRatio2 = reader.ReadSingle();
                float rotationsGearUp2 = reader.ReadSingle();
                float rotationsGearDown2 = reader.ReadSingle();
                float gearRatio3 = reader.ReadSingle();
                float rotationsGearUp3 = reader.ReadSingle();
                float rotationsGearDown3 = reader.ReadSingle();
                float gearRatio4 = reader.ReadSingle();
                float rotationsGearUp4 = reader.ReadSingle();
                float rotationsGearDown4 = reader.ReadSingle();
                float gearRatio5 = reader.ReadSingle();
                float rotationsGearUp5 = reader.ReadSingle();
                float rotationsGearDown5 = reader.ReadSingle();
                float gearRatio6 = reader.ReadSingle();
                float rotationsGearUp6 = reader.ReadSingle();
                float rotationsGearDown6 = reader.ReadSingle();
                float minClutchGlobal = reader.ReadSingle();
                float minClutchAngleCoefficientGlobal = reader.ReadSingle();
                float shiftDelayMin = reader.ReadSingle();
                float shiftDelayMax = reader.ReadSingle();

                var item = new CarGearBoxesItem()
                {
                    Id = id,
                    Description = description,
                    Automatic = automatic,
                    GearCount = gearCount,
                    GearReverseCount = gearReverseCount,
                    GearRatio0 = gearRatio0,
                    RotationsGearUp0 = rotationsGearUp0,
                    RotationsGearDown0 = rotationsGearDown0,
                    GearRatio1 = gearRatio1,
                    RotationsGearUp1 = rotationsGearUp1,
                    RotationsGearDown1 = rotationsGearDown1,
                    GearRatio2 = gearRatio2,
                    RotationsGearUp2 = rotationsGearUp2,
                    RotationsGearDown2 = rotationsGearDown2,
                    GearRatio3 = gearRatio3,
                    RotationsGearUp3 = rotationsGearUp3,
                    RotationsGearDown3 = rotationsGearDown3,
                    GearRatio4 = gearRatio4,
                    RotationsGearUp4 = rotationsGearUp4,
                    RotationsGearDown4 = rotationsGearDown4,
                    GearRatio5 = gearRatio5,
                    RotationsGearUp5 = rotationsGearUp5,
                    RotationsGearDown5 = rotationsGearDown5,
                    GearRatio6 = gearRatio6,
                    RotationsGearUp6 = rotationsGearUp6,
                    RotationsGearDown6 = rotationsGearDown6,
                    MinClutchGlobal = minClutchGlobal,
                    MinClutchAngleCoefficientGlobal = minClutchAngleCoefficientGlobal,
                    ShiftDelayMin = shiftDelayMin,
                    ShiftDelayMax = shiftDelayMax
                };

                items[i] = item;
            }

            return new CarGearBoxesTable()
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

            foreach(CarGearBoxesItem gearbox in Items)
            {
                writer.Write(gearbox.Id);
                writer.WriteStringBuffer(32, gearbox.Description);
                writer.Write(gearbox.Automatic);
                writer.Write(gearbox.GearCount);
                writer.Write(gearbox.GearReverseCount);
                writer.Write(gearbox.GearRatio0);
                writer.Write(gearbox.RotationsGearUp0);
                writer.Write(gearbox.RotationsGearDown0);
                writer.Write(gearbox.GearRatio1);
                writer.Write(gearbox.RotationsGearUp1);
                writer.Write(gearbox.RotationsGearDown1);
                writer.Write(gearbox.GearRatio2);
                writer.Write(gearbox.RotationsGearUp2);
                writer.Write(gearbox.RotationsGearDown2);
                writer.Write(gearbox.GearRatio3);
                writer.Write(gearbox.RotationsGearUp3);
                writer.Write(gearbox.RotationsGearDown3);
                writer.Write(gearbox.GearRatio4);
                writer.Write(gearbox.RotationsGearUp4);
                writer.Write(gearbox.RotationsGearDown4);
                writer.Write(gearbox.GearRatio5);
                writer.Write(gearbox.RotationsGearUp5);
                writer.Write(gearbox.RotationsGearDown5);
                writer.Write(gearbox.GearRatio6);
                writer.Write(gearbox.RotationsGearUp6);
                writer.Write(gearbox.RotationsGearDown6);
                writer.Write(gearbox.MinClutchGlobal);
                writer.Write(gearbox.MinClutchAngleCoefficientGlobal);
                writer.Write(gearbox.ShiftDelayMin);
                writer.Write(gearbox.ShiftDelayMax);
            }
        }
    }
}
