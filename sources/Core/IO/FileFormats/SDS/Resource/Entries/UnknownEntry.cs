using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class UnknownEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        var resourceDescriptors = ManifestEntryDescriptors.CreateEmpty();
        resourceDescriptors.AddFileName(name);

        return new EntryDeserializeResult
        {
            ManifestEntryDescriptors = resourceDescriptors,
            DataDescriptors = new[] { new DataDescriptor(name, resourceEntry.Data!) } // TODO fix this nullable data
        };
    }

    public static EntrySerializeResult Serialize(
        ManifestEntry manifestEntry,
        string path,
        Endian endian
    )
    {
        string filename = manifestEntry.Descriptors.GetFilename()!;
        string pathToRead = Path.Join(path, filename);
        byte[] data = File.ReadAllBytes(pathToRead);

        return new EntrySerializeResult
        {
            DataDescriptor = filename,
            ResourceEntry = new ResourceEntry
            {
                Version = manifestEntry.MetaData.Version,
                TypeId = manifestEntry.MetaData.Type.Id,
                FileHash = manifestEntry.MetaData.FileHash, // TODO compute that
                Data = data,
                SlotRamRequired = manifestEntry.MetaData.SlotRamRequired,
                SlotVramRequired = manifestEntry.MetaData.SlotVramRequired,
                OtherRamRequired = manifestEntry.MetaData.OtherRamRequired,
                OtherVramRequired = manifestEntry.MetaData.OtherVramRequired
            }
        };
    }
}