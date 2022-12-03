using Core.Games;
using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities;

public class CitiesTableItem
{
    public string? Unk0 { get; init; }
    public uint? Unk1 { get; init; } 
    public uint Id { get; init; }
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;
    public XBinHash64Name TextId { get; init; } = null!;
    public XBinHash64Name CarGarageType { get; init; } = null!;
    public XBinHash64Name BoatGarageType { get; init; } = null!;
    public string Map { get; init; } = null!;
    public CityRespawnPlace[] CityRespawnPlaces { get; init; } = null!;
    public XBinVector2[] CityPoints { get; private set; } = null!;
    public CityPolygon[] CityPolygons { get; private set; } = null!;

    private CitiesTableItem()
    {
    }

    public static CitiesTableItem ReadFromFile(BinaryReader reader, GamesEnumerator gameVersion)
    {
        reader.ReadUInt32(); // offset
        uint cityRespawnPlacesCount0 = reader.ReadUInt32();
        uint cityRespawnPlacesCount1 = reader.ReadUInt32();
        
        if (cityRespawnPlacesCount0 != cityRespawnPlacesCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {cityRespawnPlacesCount0}, Count1 = {cityRespawnPlacesCount1}"
            );
        }
        
        var lll = reader.ReadUInt32(); // Maybe CityAreas offset?
      
        uint id = reader.ReadUInt32();
        string name = reader.ReadStringPointerWithOffset();
        string missionLine = reader.ReadStringPointerWithOffset();
        string sdsPrefix = reader.ReadStringPointerWithOffset();
        XBinHash64Name textId = XBinHash64Name.ReadFromFile(reader);
        XBinHash64Name carGarageType = XBinHash64Name.ReadFromFile(reader);
        XBinHash64Name boatGarageType = XBinHash64Name.ReadFromFile(reader);

        string unk0 = null!;

        if (gameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
        {
            unk0 = reader.ReadStringPointerWithOffset();
        }

        string map = reader.ReadStringPointerWithOffset();
        
        uint? unk1 = null!;

        // Content doesn't exist in M1DE.. Unused?
        if (gameVersion == GamesEnumerator.Mafia3)
        {
            unk1 = reader.ReadUInt32();
        }

        return new CitiesTableItem()
        {
            Unk0 = unk0,
            Unk1 = unk1,
            Id = id,
            Name = name,
            MissionLine = missionLine,
            SdsPrefix = sdsPrefix,
            TextId = textId,
            CarGarageType = carGarageType,
            BoatGarageType = boatGarageType,
            Map = map,
            CityRespawnPlaces = new CityRespawnPlace[cityRespawnPlacesCount0],
        };
    }

    public void WriteToFile(XBinWriter writer, GamesEnumerator gameVersion)
    {
        if (CityRespawnPlaces.Length > 0)
        {
            writer.PushObjectPointer($"CityRespawnPlacesOffset_{Id}");
        }
        else
        {
            // Since array is empty, offset should be equal to zero
            writer.Write(0);
        }
        
        writer.Write(CityRespawnPlaces.Length);
        writer.Write(CityRespawnPlaces.Length);
        writer.PushObjectPointer($"CityAreasOffset_{Id}");

        writer.Write(Id);
        writer.PushStringPointer(Name);
        writer.PushStringPointer(MissionLine);
        writer.PushStringPointer(SdsPrefix);
        TextId.WriteToFile(writer);
        CarGarageType.WriteToFile(writer);
        BoatGarageType.WriteToFile(writer);

        if (gameVersion == GamesEnumerator.Mafia1DefinitiveEdition)
        {
            writer.PushStringPointer(Unk0!);
        }

        writer.PushStringPointer(Map);

        // Content doesn't exist in M1DE.. Unused?
        if (gameVersion == GamesEnumerator.Mafia3)
        {
            writer.Write(Unk1!.Value);
        }
    }

    public void ReadRespawnPlaces(BinaryReader reader)
    {
        for(var i = 0; i < CityRespawnPlaces.Length; i++)
        {
            CityRespawnPlaces[i] = CityRespawnPlace.ReadFromFile(reader);
        }

        reader.ReadUInt32(); // offset
        uint cityPointsCount0 = reader.ReadUInt32();
        uint cityPointsCount1 = reader.ReadUInt32();
        
        if (cityPointsCount0 != cityPointsCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {cityPointsCount0}, Count1 = {cityPointsCount1}"
            );
        }
        
        reader.ReadUInt32(); // offset
        uint cityPolygonsCount0 = reader.ReadUInt32();
        uint cityPolygonsCount1 = reader.ReadUInt32();
        
        if (cityPolygonsCount0 != cityPolygonsCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {cityPolygonsCount0}, Count1 = {cityPolygonsCount1}"
            );
        }

        CityPoints = new XBinVector2[cityPointsCount0];
        
        for(var x = 0; x < CityPoints.Length; x++)
        {
            CityPoints[x] = XBinVector2 .ReadFromFile(reader);
        }

        CityPolygons = new CityPolygon[cityPolygonsCount0];
        
        for (var x = 0; x < CityPolygons.Length; x++)
        {
            CityPolygons[x] = CityPolygon.ReadFromFile(reader);
        }
    }

    public void WriteRespawnPlaces(XBinWriter writer)
    {
        writer.FixUpObjectPointer($"CityRespawnPlacesOffset_{Id}");
        
        foreach (CityRespawnPlace cityRespawnPlace in CityRespawnPlaces)
        {
            cityRespawnPlace.PlayerPosition.WriteToFile(writer);
            cityRespawnPlace.PlayerDirection.WriteToFile(writer);
            writer.Write((int)cityRespawnPlace.RespawnType);
            writer.PushStringPointer(cityRespawnPlace.StreamMapPart);
        }

        writer.FixUpObjectPointer($"CityAreasOffset_{Id}");

        
        if (CityPoints.Length > 0)
        {
            writer.PushObjectPointer("CityPointsOffset");
        }
        else
        {
            // Since array is empty, offset should be equal to zero
            writer.Write(0);
        }
        
        writer.Write(CityPoints.Length);
        writer.Write(CityPoints.Length);
        
        if (CityPolygons.Length > 0)
        {
            writer.PushObjectPointer("CityPolygonsOffset");
        }
        else
        {
            // Since array is empty, offset should be equal to zero
            writer.Write(0);
        }
        
        writer.Write(CityPolygons.Length);
        writer.Write(CityPolygons.Length);

        writer.FixUpObjectPointer("CityPointsOffset");
        
        foreach(XBinVector2 entry in CityPoints)
        {
            entry.WriteToFile(writer);
        }

        writer.FixUpObjectPointer("CityPolygonsOffset");

        foreach (CityPolygon entry in CityPolygons)
        {
            if (entry.Indexes.Length > 0)
            {
                writer.PushObjectPointer("CityPolygonsIndexesOffset");
            }
            else
            {
                // Since array is empty, offset should be equal to zero
                writer.Write(0);
            }

            writer.Write(entry.Indexes.Length);
            writer.Write(entry.Indexes.Length);

            writer.PushStringPointer(entry.Name);
            entry.TextId.WriteToFile(writer);
            writer.Write(entry.Unk0);

            writer.FixUpObjectPointer("CityPolygonsIndexesOffset");
            
            foreach (ushort index in entry.Indexes)
            {
                writer.Write(index);
            }
        }
    }

    public override string ToString()
    {
        return $"ID: {Id} - {Name}";
    }
}