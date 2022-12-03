using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

//also known as SystemObjectDatabase
public class XBinEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name, // TODO rename this
        Endian endian
    )
    {
        XBinResource resource;
        
        using (var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = XBinResource.Deserialize(resourceEntry.Version, stream, endian);
        }

        name = resource.Name; // TODO Check if extension or path is valid

        int dataCount = resource.XmlData is not null ? 2 : 1;
        var data = new DataDescriptor[dataCount];
        data[0] = new DataDescriptor(name, resource.Data);
        
        ManifestEntryDescriptors resourceDescriptors = ManifestEntryDescriptors.FromResource(resource);
        resourceDescriptors.AddFileName(name);

        // ReSharper disable once InvertIf
        if (resource.XmlData is not null)
        {
            string xmlFileName = name + ".xml";
            resourceDescriptors.Descriptors.Add("XmlFile", xmlFileName);
            data[1] = new DataDescriptor(xmlFileName, resource.XmlData);
        }

        return new EntryDeserializeResult
        {
            ManifestEntryDescriptors = resourceDescriptors,
            DataDescriptors = data
        };
    }

    public static EntrySerializeResult Serialize(
        ManifestEntry manifestEntry,
        string path,
        Endian endian
    )
    {
        var resource = manifestEntry.Descriptors.ToResource<XBinResource>();
        string filename = manifestEntry.Descriptors.GetFilename()!;
        ushort version = manifestEntry.MetaData.Version;
        
        string pathToRead = Path.Join(path, filename);
        resource.Data = File.ReadAllBytes(pathToRead);
        resource.Size = resource.Data.Length;
        
        pathToRead = Path.Join(path, filename + ".xml");

        if (File.Exists(pathToRead))
        {
            resource.XmlData = File.ReadAllBytes(pathToRead);
        }

        var resourceEntry = new ResourceEntry
        {
            Version = manifestEntry.MetaData.Version,
            TypeId = manifestEntry.MetaData.Type.Id,
            FileHash = manifestEntry.MetaData.FileHash, // TODO compute that
            SlotRamRequired = manifestEntry.MetaData.SlotRamRequired // TODO find correct value
        };

        using (var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            resourceEntry.Data = stream.ToArray();
        }

        return new EntrySerializeResult
        {
            DataDescriptor = "not available",
            ResourceEntry = resourceEntry
        };
    }
}