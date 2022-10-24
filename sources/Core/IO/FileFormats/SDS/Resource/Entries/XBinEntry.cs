using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

//also known as SystemObjectDatabase
public class XBinEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        XBinResource resource;
        
        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = XBinResource.Deserialize(entry.Version, stream, endian);
        }

        writer.WriteElementString("File", resource.Name);
        writer.WriteElementString("Unk01", resource.Unk01.ToString());
        writer.WriteElementString("Unk02", resource.Unk02.ToString());
        writer.WriteElementString("Unk03", resource.Unk03.ToString());
        writer.WriteElementString("Unk04", resource.Unk04);
        writer.WriteElementString("Hash", resource.Hash.ToString());
        writer.WriteElementString("Version", entry.Version.ToString());
        
        string pathToWrite = Path.Join(path, resource.Name);
        Directory.CreateDirectory(Path.GetDirectoryName(pathToWrite)!);
        File.WriteAllBytes(pathToWrite, resource.Data);

        if (resource.XmlData is not null)
        {
            string pathToWriteForXml = Path.Join(path, resource.Name + ".xml");
            Directory.CreateDirectory(Path.GetDirectoryName(pathToWriteForXml)!);
            File.WriteAllBytes(pathToWriteForXml, resource.XmlData);
        }

        writer.WriteEndElement();
        return resource.Name;
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
        
        var resource = new XBinResource();

        //read contents from XML entry
        nodes.Current.MoveToNext();
        resource.Name = nodes.Current.Value;
        nodes.Current.MoveToNext();
        resource.Unk01 = Convert.ToUInt64(nodes.Current.Value);
        nodes.Current.MoveToNext();
        resource.Unk02 = Convert.ToUInt32(nodes.Current.Value);
        nodes.Current.MoveToNext();
        resource.Unk03 = Convert.ToUInt64(nodes.Current.Value);
        nodes.Current.MoveToNext();
        resource.Unk04 = nodes.Current.Value;
        nodes.Current.MoveToNext();
        resource.Hash = Convert.ToUInt64(nodes.Current.Value);
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);

        //finish
        string pathToRead = Path.Join(path, resource.Name);
        resource.Data = File.ReadAllBytes(pathToRead);
        resource.Size = resource.Data.Length;
        sourceDataDescriptionNode.InnerText = resource.Name;

        // Allocate size of XBin to resource header
        entry.SlotRamRequired = (uint)(resource.Size);

        using var stream = new MemoryStream();
        resource.Serialize(entry.Version, stream, endian);
        entry.Data = stream.ToArray();
        return entry;
    }
}