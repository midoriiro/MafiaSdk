using Core.Games;
using Core.IO.ResourceFormats.XBin.Types.Vehicles.Mafia1;

namespace Core.IO.ResourceFormats.XBin.Types.Vehicles
{
    public class VehicleTable : ITable
    {
        public uint Unk0 { get; init; }
        public GamesEnumerator GameVersion { get; init; }
        public List<object> Items { get; init; } = null!; // TODO replace object by proper type

        private VehicleTable()
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
            
            var items = new object[count0];
            GamesEnumerator gameVersion = GameWorkSpace.Instance().SelectedGame.Type;
            
            for (var i = 0; i < count1; i++)
            {
                items[i] = gameVersion switch
                {
                    GamesEnumerator.Mafia1DefinitiveEdition => VehicleTableItem.ReadEntry(reader),
                    GamesEnumerator.Mafia3 => Mafia3.VehicleTableItem.ReadEntry(reader),
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(gameVersion),
                        "Could not determine vehicle table type"
                    )
                };
            }

            return new VehicleTable()
            {
                Unk0 = unk0,
                GameVersion = gameVersion,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount= Items.Count;
            
            writer.Write(Unk0);
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            foreach(IVehicleTableItem vehicle in Items)
            {
                vehicle.WriteEntry(writer);
            }
        }
    }
}
