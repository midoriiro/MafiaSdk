using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.HumanWeaponImpact
{
    public class HumanWeaponImpactTable : ITable
    {
        public int Unk0 { get; init; }
        public List<ParticleNamesItem> Items { get; init; } = null!;

        private HumanWeaponImpactTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            int unk0 = reader.ReadInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new ParticleNamesItem[count1];
            
            for(var i = 0; i < items.Length; i++)
            {
                XBinHash64Name initialShot = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name headshot = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name grazingShot = XBinHash64Name.ReadFromFile(reader);
                XBinHash64Name killShot = XBinHash64Name.ReadFromFile(reader);
                float splashDiameter = reader.ReadSingle();
                float splashHardness = reader.ReadSingle();
                float splashStrength = reader.ReadSingle();
                float shotDiameter = reader.ReadSingle();
                float shotHardness = reader.ReadSingle();
                float shotStrength = reader.ReadSingle();
                
                items[i] = new ParticleNamesItem()
                {
                    InitialShot = initialShot,
                    Headshot = headshot,
                    GrazingShot = grazingShot,
                    KillShot = killShot,
                    SplashDiameter = splashDiameter,
                    SplashHardness = splashHardness,
                    SplashStrength = splashStrength,
                    ShotDiameter = shotDiameter,
                    ShotHardness = shotHardness,
                    ShotStrength = shotStrength
                };
            }

            return new HumanWeaponImpactTable()
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

            for(var i = 0; i < itemsCount; i++)
            {
                ParticleNamesItem item = Items[i];
                item.InitialShot.WriteToFile(writer);
                item.Headshot.WriteToFile(writer);
                item.GrazingShot.WriteToFile(writer);
                item.KillShot.WriteToFile(writer);
                writer.Write(item.SplashDiameter);
                writer.Write(item.SplashHardness);
                writer.Write(item.SplashStrength);
                writer.Write(item.ShotDiameter);
                writer.Write(item.ShotHardness);
                writer.Write(item.ShotStrength);
            }
        }
    }
}
