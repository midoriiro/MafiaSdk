using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Caches;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

// We do not actually use this yet; We just use it to get the names.
public class HavokEntry : IResourceEntry
{
    private static readonly ResourceNameCache FileNamesAndHash = ResourceNameCache.FromFile(
        "Resources/Caches/M3DE_ResourceNameDatabase.txt"    
    );
    
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        HavokResource resource;

        using(var stream = new MemoryStream(entry.Data!))
        {
            resource = HavokResource.Deserialize(entry.Version, stream, endian);
            entry.Data = resource.Data;
        }

        // If not correctly named - See if its toolkit standard 'File_'.
        // If yes, see if we can grab the name from our DB and apply hkx extension.
        if(FileNamesAndHash.ContainsKey(resource.FileHash) && name.Contains("File_"))
        {
            name = FileNamesAndHash[resource.FileHash];
            name += ".hkx";
        }
        
        // Construct path
        Directory.CreateDirectory(Path.GetDirectoryName(Path.Join(path, name))!);

        writer.WriteElementString("File", name);
        writer.WriteElementString("Unk01", resource.Unk01.ToString());
        writer.WriteElementString("FileHash", resource.FileHash.ToString());
        writer.WriteElementString("Unk02", resource.Unk02.ToString());
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
        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }
        
        var resource = new HavokResource();

        // Read contents from SDSContent.xml
        nodes.Current.MoveToNext();
        string name = nodes.Current.Value;
        nodes.Current.MoveToNext();
        uint unk01 = uint.Parse(nodes.Current.Value);
        nodes.Current.MoveToNext();
        ulong fileHash = ulong.Parse(nodes.Current.Value);
        nodes.Current.MoveToNext();
        uint unk02 = uint.Parse(nodes.Current.Value);
        nodes.Current.MoveToNext();
        ushort version = ushort.Parse(nodes.Current.Value);

        // Serialize into separate stream
        using (var stream = new MemoryStream())
        {
            resource.Unk01 = unk01;
            resource.FileHash = fileHash;
            resource.Unk02 = unk02;

            string pathToRead = Path.Join(path, name);
            resource.Data = File.ReadAllBytes(pathToRead);
            
            resource.Serialize(entry.Version, stream, endian);

            // Set entry information and this is it.
            entry.Data = stream.ToArray();
            entry.Version = version;
        }

        sourceDataDescriptionNode.InnerText = "Not Available";

        return entry;
    }
}