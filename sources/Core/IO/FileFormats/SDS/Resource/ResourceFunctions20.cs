using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Entries;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource;

public static class ResourceFunction20
{
    public static DeserializeResult DeserializeResources(
        ArchiveFile archiveFile
    )
    {
        var dataToWrite = new List<DataDescriptor>();
        var manifestEntries = new List<ManifestEntry>();
        
        for (var index = 0; index < archiveFile.ResourceEntries.Count; index++)
        {
            ResourceEntry entry = archiveFile.ResourceEntries[index];

            EntryDeserializeResult result;
            string nameToPass = archiveFile.ResourceNames[index];
            string resourceTypeName = entry.TypeId != -1 ? archiveFile.ResourceTypes[entry.TypeId].Name : "UnknownType";

            switch (resourceTypeName)
            {
                case "Texture":
                    result = TextureEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "hkAnimation":
                    result = HavokAnimationEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "NAV_PATH_DATA":
                    result = HavokNavigationEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "NAV_AIWORLD_DATA":
                case "RoadMap":
                case "EnlightenResource":
                    result = BasicEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Generic":
                    result = GenericEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "MemFile":
                    result = MemFileEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "SystemObjectDatabase":
                    result = XBinEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "XML":
                    result = XmlEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Flash":
                    result = FlashEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Script":
                    result = ScriptEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Cutscene":
                    result = CutsceneEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "UnknownType":
                    result = UnknownEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown resource type: {resourceTypeName}");
            }

            ManifestEntryMetaData entryMetaData = ManifestEntryMetaData.From(
                entry, 
                entry.TypeId != -1 ? archiveFile.ResourceTypes[entry.TypeId] : null
            );

            manifestEntries.Add(new ManifestEntry
            {
                MetaData = entryMetaData,
                Descriptors = result.ManifestEntryDescriptors
            });
            
            dataToWrite.AddRange(result.DataDescriptors);
        }

        return new DeserializeResult
        {
            Manifest = Manifest.Manifest.From(archiveFile, manifestEntries),
            DataDescriptors = dataToWrite
        };
    }

    public static ArchiveFile SerializeResources(
        Manifest.Manifest manifest,
        string path
    )
    {
        var results = new List<EntrySerializeResult>();

        foreach (ManifestEntry manifestEntry in manifest.Entries)
        {
            EntrySerializeResult result;

            string resourceTypeName = manifestEntry.MetaData.Type.Name;

            switch (resourceTypeName)
            {
                case "Texture":
                    result = TextureEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "hkAnimation":
                    result = HavokAnimationEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "NAV_PATH_DATA":
                    result = HavokNavigationEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "NAV_AIWORLD_DATA":
                case "RoadMap":
                case "EnlightenResource":
                    result = BasicEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Generic":
                    result = GenericEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "MemFile":
                    result = MemFileEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "SystemObjectDatabase":
                    result = XBinEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "XML":
                    result = XmlEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Flash":
                    result = FlashEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Script":
                    result = ScriptEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Cutscene":
                    result = CutsceneEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "UnknownType":
                    result = UnknownEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown resource type: {resourceTypeName}");
            }
            
            results.Add(result);
        }

        return ArchiveFile.From(manifest, results);
    }
}