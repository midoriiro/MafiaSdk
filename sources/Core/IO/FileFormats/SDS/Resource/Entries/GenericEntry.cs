using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class GenericEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        GenericResource resource;

        // Read generic resource
        using(var stream = new MemoryStream(entry.Data!))
        {
            resource = GenericResource.Deserialize(entry.Version, stream, endian);
        }
        
        name = resource.DetermineName(entry, name);

        // Construct the path and attempt to save the data.
        string pathToWrite = Path.Join(path, name);
        Directory.CreateDirectory(Path.GetDirectoryName(pathToWrite)!);
        File.WriteAllBytes(pathToWrite, resource.Data);

        // Write to SDSContent.
        writer.WriteElementString("File", name);
        writer.WriteElementString("Generic_Unk01", resource.Unk0.ToString());
        writer.WriteElementString("Version", entry.Version.ToString());
        writer.WriteEndElement();
        return name;
    }

    public static ResourceEntry Write(
        ResourceEntry entry, 
        XPathNodeIterator nodes, 
        XmlNode sourceDataDescriptionNode,
        string path, 
        Endian endian
    )
    {
        // TODO create method to ensure current is always not null when calling MoveToNext() method
        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }
        
        // Create new resource
        var resource = new GenericResource();

        // Fetch data from XML
        nodes.Current.MoveToNext();
        resource.DebugName = nodes.Current.Value;
        nodes.Current.MoveToNext();
        resource.Unk0 = (ushort)nodes.Current.ValueAsInt;
        nodes.Current.MoveToNext();
        entry.Version = (ushort)nodes.Current.ValueAsInt;

        // Read data and serialize into the resource format.
        string pathToRead = Path.Join(path, resource.DebugName);
        resource.Data = File.ReadAllBytes(pathToRead);
        
        using(var stream = new MemoryStream())
        {
            resource.Serialize(entry.Version, stream, endian);
            entry.Data = stream.ToArray();
        }

        int extensionStart = resource.DebugName.IndexOf(".", StringComparison.Ordinal);
        string filename = resource.DebugName.Remove(extensionStart);
        sourceDataDescriptionNode.InnerText = "Not Available";

        return entry;
    }
}