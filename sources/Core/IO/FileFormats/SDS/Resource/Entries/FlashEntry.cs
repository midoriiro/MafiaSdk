using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class FlashEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path,
        Endian endian
    )
    {
        // Read the resource first; we have the nicety of having the flash name stored 
        // in the meta info.
        FlashResource resource;
        
        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = FlashResource.Deserialize(entry.Version, stream, endian);
            entry.Data = resource.Data;
        }

        // Since we know that flash will have the filename, 
        // we collect it from here instead.
        if(string.IsNullOrEmpty(name))
        {
            name = resource.FileName;
        }

        Directory.CreateDirectory(Path.Join(path, Path.GetDirectoryName(name)!));
        
        writer.WriteElementString("File", name);
        writer.WriteElementString("Name", resource.Name);

        // In this case this is valid; we will no doubt get a name.
        return name;
    }

    public static ResourceEntry Write(
        ResourceEntry entry, 
        XPathNodeIterator nodes, 
        XmlNode sourceDataDescriptionNode, 
        string path,
        Endian endian)
    {
        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }
        
        var resource = new FlashResource();

        //read contents from XML entry
        nodes.Current.MoveToNext();
        resource.FileName = nodes.Current.Value;
        nodes.Current.MoveToNext();
        resource.Name = nodes.Current.Value;
        nodes.Current.MoveToNext();
        entry.Version = (ushort)nodes.Current.ValueAsInt;

        string pathToRead = Path.Join(path, resource.FileName);
        resource.Data = File.ReadAllBytes(pathToRead);

        using (var stream = new MemoryStream())
        {
            resource.Serialize(entry.Version, stream, endian);
            entry.Data = stream.ToArray();
        }

        sourceDataDescriptionNode.InnerText = resource.FileName;
        return entry;
    }
}