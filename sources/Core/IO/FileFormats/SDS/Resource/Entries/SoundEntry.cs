

//THIS ISN'T GIBBED. BUT STILL USES GIBBED STUFF :)

using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Entries.Extensions;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class SoundEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        SoundResource resource;

        using (var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = SoundResource.Deserialize(resourceEntry.Version, stream, endian);
        }
        
        name += ".fsb";

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
        var resource = manifestEntry.Descriptors.ToResource<SoundResource>();
        string filename = manifestEntry.Descriptors.GetFilename()!;
        ushort version = manifestEntry.MetaData.Version;
        
        string pathToRead = Path.Join(path, filename);
        resource.Data = File.ReadAllBytes(pathToRead);
        resource.FileSize = resource.Data.Length;

        var resourceEntry = new ResourceEntry
        {
            Version = manifestEntry.MetaData.Version,
            TypeId = manifestEntry.MetaData.Type.Id,
            FileHash = manifestEntry.MetaData.FileHash, // TODO compute that
            SlotRamRequired = manifestEntry.MetaData.SlotRamRequired, // TODO find correct value
            SlotVramRequired = (uint)resource.FileSize
        };

        using (var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            resourceEntry.Data = stream.ToArray();
        }

        return new EntrySerializeResult
        {
            DataDescriptor = filename.RemoveExtension(),
            ResourceEntry = resourceEntry
        };
    }
}