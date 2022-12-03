using System.Collections.Immutable;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Entries;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource;

public static class ResourceFunction19
{
    private static readonly ImmutableDictionary<string, string> ResourceTypeExtension = new Dictionary<string, string>
    {
        { "IndexBufferPool", "ibp" },
        { "VertexBufferPool", "vbp" },
        { "AnimalTrafficPaths", "atp" },
        { "FrameResource", "frs" },
        { "Effects", "eff" },
        { "FrameNameTable", "fnt" },
        { "EntityDataStorage", "eds" },
        { "PREFAB", "prefab" },
        { "ItemDesc", "ids" },        
        { "Actors", "actors" },
        { "Collisions", "collisions" },
        { "SoundTable", "stbl" },
        { "Speech", "speech" },
        { "FxAnimSet", "fxs" },
        { "FxActor", "fxa" },
        { "Cutscene", "cutscene" },
        { "Translokator", "translokator" },
        { "Animation2", "a2" },
        { "NAV_AIWORLD_DATA", "nad" },
        { "NAV_OBJ_DATA", "nod" },
        { "NAV_HPD_DATA", "nhd" },
        { "Animated Texture", "ant" }
    }.ToImmutableDictionary();

    public static DeserializeResult DeserializeResources(
        ArchiveFile archiveFile
    )
    {
        var dataToWrite = new List<DataDescriptor>();
        var manifestEntries = new List<ManifestEntry>();
        
        for (var index = 0; index != archiveFile.ResourceEntries.Count; index++)
        {
            ResourceEntry entry = archiveFile.ResourceEntries[index];

            EntryDeserializeResult result;
            string nameToPass = archiveFile.ResourceNames[index];
            string resourceTypeName = entry.TypeId != -1 ? archiveFile.ResourceTypes[entry.TypeId].Name : "UnknownType";

            switch (resourceTypeName)
            {
                case "IndexBufferPool":
                case "VertexBufferPool":
                case "AnimalTrafficPaths":
                case "FrameResource":
                case "Effects":
                case "FrameNameTable":
                case "EntityDataStorage":
                case "PREFAB":
                case "ItemDesc":
                case "Actors":
                case "Collisions":
                case "SoundTable":
                case "Speech":
                case "FxAnimSet":
                case "FxActor":
                case "Cutscene":
                case "Translokator":
                case "Animation2":
                case "NAV_AIWORLD_DATA":
                case "NAV_OBJ_DATA":
                case "NAV_HPD_DATA":
                case "Animated Texture":
                    nameToPass = $"{nameToPass}.{ResourceTypeExtension[resourceTypeName]}";
                    result = BasicEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Texture":
                    result = TextureEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Mipmap":
                    result = TextureMipMapEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "AudioSectors":
                    result = AudioSectorEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Script":
                    result = ScriptEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "XML":
                    result = XmlEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Sound":
                    result = SoundEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "MemFile":
                    result = MemFileEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
                    break;
                case "Table":
                    result = TableEntry.Deserialize(entry, nameToPass, archiveFile.Endian);
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
                case "FrameResource":
                case "Effects":
                case "PREFAB":
                case "ItemDesc":
                case "FrameNameTable":
                case "Actors":
                case "NAV_AIWORLD_DATA":
                case "NAV_OBJ_DATA":
                case "NAV_HPD_DATA":
                case "Cutscene":
                case "FxActor":
                case "FxAnimSet":
                case "Translokator":
                case "Speech":
                case "SoundTable":
                case "AnimalTrafficPaths":
                    result = BasicEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "AudioSectors":
                    result = AudioSectorEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Animated Texture":
                    result = AnimatedTextureEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Collisions":
                    result = CollisionEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "IndexBufferPool":
                case "VertexBufferPool":
                    result = BufferEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "EntityDataStorage":
                    result = EntityDataEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Animation2":
                    result = AnimationEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Texture":
                    result = TextureEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Mipmap":
                    result = TextureMipMapEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Sound":
                    result = SoundEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "XML":
                    result = XmlEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "MemFile":
                    result = MemFileEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Script":
                    result = ScriptEntry.Serialize(manifestEntry, path, manifest.Endian);
                    break;
                case "Table":
                    result = TableEntry.Serialize(manifestEntry, path, manifest.Endian);
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