using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class HavokNavigationEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        HavokNavigationResource resource;

        using(var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = HavokNavigationResource.Deserialize(resourceEntry.Version, stream, endian);
        }

        ManifestEntryDescriptors resourceDescriptors = ManifestEntryDescriptors.FromResource(resource);
        resourceDescriptors.AddFileName(name);

        return new EntryDeserializeResult
        {
            ManifestEntryDescriptors = resourceDescriptors,
            DataDescriptors = new[] { new DataDescriptor(name, resource.Data) }
        };
    }

    public static EntrySerializeResult Serialize(
        ManifestEntry manifestEntry,
        string path,
        Endian endian
    )
    {
        var resource = manifestEntry.Descriptors.ToResource<HavokNavigationResource>();
        string filename = manifestEntry.Descriptors.GetFilename()!;
        ushort version = manifestEntry.MetaData.Version;
        
        string pathToRead = Path.Join(path, filename);
        resource.Data = File.ReadAllBytes(pathToRead);

        var resourceEntry = new ResourceEntry
        {
            Version = manifestEntry.MetaData.Version,
            TypeId = manifestEntry.MetaData.Type.Id,
            FileHash = manifestEntry.MetaData.FileHash // TODO compute that
        };
        
        using (var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            resourceEntry.Data = stream.ToArray();
        }

        resourceEntry.SlotRamRequired = manifestEntry.MetaData.SlotRamRequired; // TODO find a solution to get correct value

        return new EntrySerializeResult
        {
            DataDescriptor = filename,
            ResourceEntry = resourceEntry
        };
    }
}