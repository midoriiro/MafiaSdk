using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

// NOTE: Only supports M1: DE and M3 Cutscene formats.
// This IS NOT for Mafia II. However it would be ideal if we could also support Mafia II.
public class CutsceneEntry : IResourceEntry
{
    public static string? Read(
        ResourceEntry entry, 
        XmlWriter writer,
        string name,
        string path, 
        Endian endian
    )
    {
        CutsceneResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = CutsceneResource.Deserialize(entry.Version, stream, endian);
        }

        // Write all EntityRecords to individual files
        writer.WriteElementString("GCRNum", resource.GcrEntityRecords.Length.ToString());
        
        foreach(CutsceneResource.GcrResource record in resource.GcrEntityRecords)
        {
            string pathToWrite = Path.Join(path, record.Name);
            Directory.CreateDirectory(Path.GetDirectoryName(pathToWrite)!);
            File.WriteAllBytes(pathToWrite, record.Content);
            writer.WriteElementString("Name", record.Name);
        }

        writer.WriteElementString("Version", entry.Version.ToString());
        writer.WriteEndElement();

        return null;
    }

    public static ResourceEntry Write(
        ResourceEntry entry, 
        XPathNodeIterator nodes, 
        XmlNode sourceDataDescriptionNode, 
        string path, 
        Endian endian
    )
    {
        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }
        
        // Read contents from SDSContent.xml
        nodes.Current.MoveToNext();
        int gcrCount = nodes.Current.ValueAsInt;
        nodes.Current.MoveToNext();

        // construct new resource
        var resource = new CutsceneResource()
        {
            GcrEntityRecords = new CutsceneResource.GcrResource[gcrCount]
        };

        for(var i = 0; i < gcrCount; i++)
        {
            string name = nodes.Current.Value;
            var record = new CutsceneResource.GcrResource();

            string combinedPath = Path.Join(path, name);
            record.Name = name;
            record.Content = File.ReadAllBytes(combinedPath);

            nodes.Current.MoveToNext();

            resource.GcrEntityRecords[i] = record;
        }

        ushort version = ushort.Parse(nodes.Current.Value);

        using (var stream = new MemoryStream())
        {
            resource.Serialize(entry.Version, stream, endian);
            entry.Data = stream.ToArray();
        }

        entry.Version = version;

        return entry;
    }
}