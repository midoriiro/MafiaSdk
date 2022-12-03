using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

// We do not actually use this yet; We just use it to get the names.
public class HavokAnimationEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        HavokAnimationResource resource;

        using(var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = HavokAnimationResource.Deserialize(resourceEntry.Version, stream, endian);
        }

        if (name.StartsWith("file_"))
        {
            string determinedName = resource.DetermineName(endian);

            if (!string.IsNullOrEmpty(determinedName))
            {
                name = determinedName; // TODO Check if extension or path is valid
            }
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
        var resource = manifestEntry.Descriptors.ToResource<HavokAnimationResource>();
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

        return new EntrySerializeResult
        {
            DataDescriptor = filename,
            ResourceEntry = resourceEntry
        };
    }
}