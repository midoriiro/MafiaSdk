using Core.Games;

namespace Core.IO.ResourceFormats.XBin.Types.CarInteriorColors
{
    public class CarInteriorColorsTable : ITable
    {
        public uint Unk0 { get; init; }
        public GamesEnumerator GameVersion { get; init; }

        public List<CarInteriorColorsItem> Items { get; init; } = null!;

        private CarInteriorColorsTable()
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

            var items = new CarInteriorColorsItem[count0];
            GamesEnumerator gameVersion = GameWorkSpace.Instance().SelectedGame.Type;

            for (var i = 0; i < count1; i++)
            {
                int carId = reader.ReadInt32();
                byte r1 = reader.ReadByte();
                byte g1 = reader.ReadByte();
                byte b1 = reader.ReadByte();
                byte r2 = reader.ReadByte();
                byte g2 = reader.ReadByte();
                byte b2 = reader.ReadByte();
                byte r3 = reader.ReadByte();
                byte g3 = reader.ReadByte();
                byte b3 = reader.ReadByte();
                byte r4 = reader.ReadByte();
                byte g4 = reader.ReadByte();
                byte b4 = reader.ReadByte();
                byte r5 = reader.ReadByte();
                byte g5 = reader.ReadByte();
                byte b5 = reader.ReadByte();

                byte? alpha = null;

                // M1:DE only.
                if (gameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
                {
                    alpha = reader.ReadByte();
                }

                int description = reader.ReadInt32();

                items[i] = new CarInteriorColorsItem()
                {
                    CarId = carId,

                    R1 = r1,
                    G1 = g1,
                    B1 = b1,

                    R2 = r2,
                    G2 = g2,
                    B2 = b2,

                    R3 = r3,
                    G3 = g3,
                    B3 = b3,

                    R4 = r4,
                    G4 = g4,
                    B4 = b4,

                    R5 = r5,
                    G5 = g5,
                    B5 = b5,

                    Alpha = alpha,
                    Description = description
                };
            }

            return new CarInteriorColorsTable()
            {
                Unk0 = unk0,
                GameVersion = gameVersion,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            foreach(CarInteriorColorsItem color in Items)
            {
                writer.Write(color.CarId);
                
                writer.Write(color.R1);
                writer.Write(color.G1);
                writer.Write(color.B1);
                
                writer.Write(color.R2);
                writer.Write(color.G2);
                writer.Write(color.B2);
                
                writer.Write(color.R3);
                writer.Write(color.G3);
                writer.Write(color.B3);
                
                writer.Write(color.R4);
                writer.Write(color.G4);
                writer.Write(color.B4);
                
                writer.Write(color.R5);
                writer.Write(color.G5);
                writer.Write(color.B5);
                
                // Only in M1:DE
                if (GameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
                {
                    writer.Write(color.Alpha!.Value);
                }

                writer.Write(color.Description);
            }
        }
    }
}
