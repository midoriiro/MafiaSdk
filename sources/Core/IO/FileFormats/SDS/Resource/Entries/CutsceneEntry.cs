using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

// NOTE: Only supports M1: DE and M3 Cutscene formats.
// This IS NOT for Mafia II. However it would be ideal if we could also support Mafia II.
public class CutsceneEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        CutsceneResource resource;

        using (var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = CutsceneResource.Deserialize(resourceEntry.Version, stream, endian);
        }
        
        ManifestEntryDescriptors resourceDescriptors = ManifestEntryDescriptors.FromResource(resource);

        var data = new DataDescriptor[resource.CutscenesCount];

        for (var index = 0; index < resource.Cutscenes.Length; index++)
        {
            CutsceneData record = resource.Cutscenes[index];
            data[index] = new DataDescriptor(record.Name, record.Data);
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
        var resource = manifestEntry.Descriptors.ToResource<CutsceneResource>();
        ushort version = manifestEntry.MetaData.Version;

        for(var index = 0; index < resource.CutscenesCount; index++)
        {
            CutsceneData cutscene = resource.Cutscenes[index];
            string pathToRead = Path.Join(path, cutscene.Name);
            cutscene.Data = File.ReadAllBytes(pathToRead);
        }

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

        resourceEntry.SlotRamRequired = manifestEntry.MetaData.SlotRamRequired; // TODO find correct value

        return new EntrySerializeResult
      {
            DataDescriptor = "not available",
            ResourceEntry = resourceEntry
        };
    }
}