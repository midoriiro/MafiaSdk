

//THIS ISN'T GIBBED. BUT STILL USES GIBBED STUFF :)

using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class SoundEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        // Create and deserialize the data.
        SoundResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = SoundResource.Deserialize(entry.Version, stream, endian);
        }

        entry.Data = resource.Data;

        // Create directories and then write the XML to finish it off.
        string fileName = name + ".fsb";
        string resourcePath = Path.Join(path, name);
        Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

        writer.WriteElementString("File", fileName);

        return fileName;
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
        
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value.Remove(nodes.Current.Value.Length - 4, 4);

        // Combine path and add extension.
        string pathToRead = Path.Join(path, file);
        pathToRead += ".fsb";

        // Get the Version and set the inner text (meta XML).
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);
        sourceDataDescriptionNode.InnerText = file;

        using var stream = new MemoryStream();
        byte[] fileData = File.ReadAllBytes(pathToRead);
        var resource = new SoundResource
        {
            Name = file,
            Data = fileData,
            FileSize = fileData.Length
        };

        resource.Serialize(entry.Version, stream, endian);

        // Fill the remaining data for the entry.
        entry.SlotRamRequired = 40;
        entry.SlotVramRequired = (uint)resource.FileSize;
        entry.Data = stream.ToArray();
        return entry;
    }
}