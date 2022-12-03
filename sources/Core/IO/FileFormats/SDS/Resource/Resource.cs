using System.Text;
using System.Xml.XPath;
using Core.Games;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Caches;
using Core.IO.FileFormats.SDS.Resource.Entries.Extensions;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Streams;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource;

public static class Resource
{
    private static readonly ResourceNameCache Mafia1TexturesCache = ResourceNameCache.LoadMafia1Textures();
    private static readonly ResourceNameCache Mafia2TexturesCache = ResourceNameCache.LoadMafia2Textures();
    private static readonly ResourceNameCache Mafia3ResourcesCache = ResourceNameCache.LoadMafia3Resources();
    
    private static readonly Dictionary<string, string> FileExtensionLookup = new()
    {
        { "Texture", ".dds" },
        { "Mipmap", ".dds" },
        { "IndexBufferPool", ".ibp" },
        { "VertexBufferPool", ".vbp" },
        { "AnimalTrafficPaths", ".atp" },
        { "FrameResource", ".fr" },
        { "Effects", ".eff" },
        { "FrameNameTable", ".fnt" },
        { "EntityDataStorage", ".eds" },
        { "PREFAB", ".prf" },
        { "ItemDesc", ".ids" },
        { "Actors", ".act" },
        { "Collisions", ".col" },
        { "SoundTable", ".stbl" },
        { "Speech", ".spe" },
        { "FxAnimSet", ".fas" },
        { "FxActor", ".fxa" },
        { "Cutscene", ".cut" },
        { "Translokator", ".tra" },
        { "Animation2", ".an2" },
        { "NAV_AIWORLD_DATA", ".nav" },
        { "NAV_OBJ_DATA", ".nov" },
        { "NAV_HPD_DATA", ".nhv" },
        { "AudioSectors", ".auds" },
        { "Script", ".luapack" },
        { "Table", ".tblpack" },
        { "Sound", ".fsb" },
        { "MemFile", ".txt" },
        { "XML", ".xml" },
        { "Animated Texture", ".ifl" }
    };

    private static readonly Dictionary<string, string> FileExtensionLookupFusion = new()
    {
        { "Texture", ".dds" },
        { "Generic", ".genr" },
        { "Flash", ".fla" },
        { "hkAnimation", ".hkx" },
        { "NAV_PATH_DATA", ".hkt" },
        { "EnlightenResource", ".enl" },
        { "RoadMap", ".gsd" },
        { "NAV_AIWORLD_DATA", ".nav" },
        { "MemFile", ".txt" },
        { "XML", ".xml" },
        { "Script", ".luapack" },
        { "SystemObjectDatabase", ".xbin" },
        { "Cutscene", ".gcs" }
    };

    private static XPathDocument? CheckForCrySds(ArchiveFile archiveFile)
    {
        int crySdsType = -1;

        for (var index = 0; index != archiveFile.ResourceTypes.Count; index++)
        {
            // check if resource type has empty name
            if (archiveFile.ResourceTypes[index].Name == "")
            {
                crySdsType = (int)archiveFile.ResourceTypes[index].Id;
            }
        }

        if (crySdsType == -1)
        {
            return null;
        }

        for (var index = 0; index < archiveFile.ResourceEntries.Count; index++)
        {
            if (archiveFile.ResourceEntries[index].TypeId != crySdsType)
            {
                continue;
            }

            // Fix for CrySDS archives
            using var stream = new MemoryStream(archiveFile.ResourceEntries[index].Data!);

            // Skip passwords
            ushort authorLen = stream.ReadValueU16();
            stream.ReadBytes(authorLen);
            int fileSize = stream.ReadValueS32();
            stream.ReadValueS32(); // password

            // pull XML and create a new document
            XPathDocument document;

            using (var reader = new StringReader(Encoding.UTF8.GetString(stream.ReadBytes(fileSize))))
            {
                document = new XPathDocument(reader);
            }

            // Remove CrySDS lock
            archiveFile.ResourceEntries.RemoveAt(index);
            archiveFile.ResourceTypes.RemoveAt(crySdsType);

            // Return document
            return document;
        }

        return null;
    }

    // TODO: Only really applicable for Fusion games, need a better solution than this.
    // I was thinking a lookup dictionary, although it already exists for SDSContent.xml.
    // Ideally, I need to unify this setup.
    private static string DetermineFileExtension(string typename, GamesEnumerator selectedGame)
    {
        // TODO: Find a new place for this.
        string extension;

        if (selectedGame is GamesEnumerator.Mafia2 or GamesEnumerator.Mafia2DefinitiveEdition)
        {
            extension = FileExtensionLookup[typename];
            return extension;
        }

        extension = FileExtensionLookupFusion[typename];
        return extension;
    }
    
    private static void DetermineTypeNames(ArchiveFile archiveFile, XPathDocument? document)
    {
        if (document is null)
        {
            return;
        }
        
        XPathNavigator nav = document.CreateNavigator();
        XPathNodeIterator nodes = nav.Select("/xml/ResourceInfo/TypeName");
        
        var index = 0;

        while (nodes.MoveNext())
        {
            if (nodes.Current is null)
            {
                continue;
            }

            string name = nodes.Current.Value;

            ResourceEntry entry = archiveFile.ResourceEntries[index];

            if (entry.TypeId == -1)
            {
                uint resourceTypeToReplace = archiveFile.ResourceTypes
                    .Single(type => type.Name == name)
                    .Id;
                entry.TypeId = (int)resourceTypeToReplace;
            }
            
            string entryTypeName = archiveFile.ResourceTypes[entry.TypeId].Name;

            if (name != entryTypeName)
            {
                // TODO ?
            }
            
            index++;
        }
    }

    private static void DetermineEntriesName(ArchiveFile archiveFile, GamesEnumerator selectedGame, XPathDocument? document)
    {
        ResourceNameCache cache = selectedGame switch
        {
            GamesEnumerator.Mafia1DefinitiveEdition => Mafia1TexturesCache,
            GamesEnumerator.Mafia2 or GamesEnumerator.Mafia2DefinitiveEdition => Mafia2TexturesCache,
            GamesEnumerator.Mafia3 => Mafia3ResourcesCache,
            _ => throw new ArgumentOutOfRangeException(nameof(selectedGame), selectedGame, null)
        };
        
        if (document is not null)
        {
            XPathNavigator nav = document.CreateNavigator();
            XPathNodeIterator nodes = nav.Select("/xml/ResourceInfo/SourceDataDescription");
            
            var index = 0;

            while (nodes.MoveNext())
            {
                if (nodes.Current is null)
                {
                    continue;
                }

                string name = nodes.Current.Value;
                
                if (name.Contains('*') || name.Contains('+'))
                {
                    string fileName = $"file_{index}";
                    name = name.Replace("*", fileName);
                    name = name.Replace("+", fileName);
                }

                if (name.Equals("not available"))
                {
                    ResourceEntry entry = archiveFile.ResourceEntries[index];

                    if (cache.ContainsKey(entry.FileHash))
                    {
                        archiveFile.ResourceNames[index] = cache[entry.FileHash];
                    }
                }
                else
                {
                    archiveFile.ResourceNames[index] = name;
                }

                index++;
            }
            
            var duplicateNames = archiveFile.ResourceNames
                .Where(name => !name.EndsWith(".dds")) // Ignore DDS textures since we change filename afterward
                .GroupBy(name => name)
                .Where(names => names.Count() > 1)
                .ToList();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var x = 0; x < duplicateNames.Count; x++)
            {
                var group = duplicateNames[x].ToList();
                int[] indexes = archiveFile.ResourceNames.FindIndexes(group);

                for (var y = 0; y < indexes.Length; y++)
                {
                    int nameIndex = indexes[y];
                    string name = archiveFile.ResourceNames[nameIndex];
                    string extension = Path.GetExtension(name);
                    string filename = Path.GetFileNameWithoutExtension(name);
                    name = $"{filename}#{y + 1}{extension}";
                    archiveFile.ResourceNames[nameIndex] = name;
                }
            }
        }
        else
        {
            // Exclude duplicate file hashes from getting proper resource names
            var resourceEntriesFileHashes = archiveFile.ResourceEntries
                .GroupBy(entry => entry.FileHash)
                .Where(entries => entries.Count() == 1)
                .Select(entries => entries.Key)
                .ToList();

            for (var index = 0; index < archiveFile.ResourceEntries.Count; index++)
            {
                ResourceEntry entry = archiveFile.ResourceEntries[index];

                if (!resourceEntriesFileHashes.Contains(entry.FileHash) || !cache.ContainsKey(entry.FileHash))
                {
                    continue;
                }

                string name = cache[entry.FileHash];
                    
                if (name.Contains('*') || name.Contains('+'))
                {
                    string fileName = $"file_{index}";
                    name = name.Replace("*", fileName);
                    name = name.Replace("+", fileName);
                }

                archiveFile.ResourceNames[index] = name;
            }
        }
    }

    private static void FillEntriesWithStubNames(ArchiveFile archiveFile, GamesEnumerator selectedGame)
    {
        for (var index = 0; index < archiveFile.ResourceEntries.Count; index++)
        {
            ResourceEntry entry = archiveFile.ResourceEntries[index];

            string fileName;

            if (entry.TypeId != -1)
            {
                // Get extension, format filename properly.
                string extension = DetermineFileExtension(archiveFile.ResourceTypes[entry.TypeId].Name, selectedGame);
                fileName = $"file_{index}{extension}";
            }
            else
            {
                fileName = "UnknownType";
            }

            archiveFile.ResourceNames.Add(fileName);
        }
    }

    // TODO refactor methods name to Deserialize/Serialize in classes: Resources, ResourcesFunctions*, *Entry
    public static void DeserializeResources(
        DataWriterScheduler dataWriterScheduler, 
        ArchiveFile archiveFile, 
        GamesEnumerator selectedGame
    )
    {
        // TODO refactor this to use another XML lib ?
        XPathDocument? document = null;

        // Pull XML from resource info XML
        // If it doesn't exist, attempt to check for CrySDS lock
        if (string.IsNullOrEmpty(archiveFile.ResourceInfoXml) == false)
        {
            using var reader = new StringReader(archiveFile.ResourceInfoXml);
            document = new XPathDocument(reader);
        }
        else if (archiveFile.Version == 19)
        {
            document = CheckForCrySds(archiveFile);
        }
        
        DetermineTypeNames(archiveFile, document);
        FillEntriesWithStubNames(archiveFile, selectedGame);
        DetermineEntriesName(archiveFile, selectedGame, document);

        DeserializeResult result = archiveFile.Version switch
        {
            19 => ResourceFunction19.DeserializeResources(archiveFile),
            20 => ResourceFunction20.DeserializeResources(archiveFile),
            _ => throw new ArgumentOutOfRangeException(
                nameof(archiveFile.Version), 
                $"Unknown version '{archiveFile.Version}'"
            )
        };

        string json = result.Manifest.Serialize();
        var manifest = new DataDescriptor("manifest", Encoding.UTF8.GetBytes(json));
        result.DataDescriptors.Add(manifest);
        dataWriterScheduler.Push(result.DataDescriptors);
    }

    // public static void DeserializePatchResources(FileInfo file, ArchiveFile archiveFile)
    // {
    //     var settings = new XmlWriterSettings
    //     {
    //         Indent = true,
    //         IndentChars = ("\t"),
    //         OmitXmlDeclaration = true
    //     };
    //
    //     // Create extraction directory
    //     string pathToSave = file.FullName;
    //     Directory.CreateDirectory(pathToSave);
    //     
    //     // Save resource
    //     string contentPath = Path.Join(pathToSave, "SDSContent.xml");
    //     var writer = XmlWriter.Create(contentPath, settings);
    //     writer.WriteStartElement("SDSResource");
    //
    //     ArchivePatchFile patchFile = null!;
    //
    //     using (FileStream input = File.OpenRead(file.FullName))
    //     {
    //         using (Stream? data = ArchiveEncryption.Unwrap(input))
    //         {
    //             patchFile = new ArchivePatchFile(file);
    //             patchFile.Deserialize(data ?? input, Endian.Little);
    //         }
    //     }
    //     
    //     var sortedResources = new Dictionary<string, Dictionary<int, string>>();
    //     var resourcePatchAvailable = new Dictionary<string, List<KeyValuePair<int, bool>>>();
    //
    //     for (var index = 0; index < archiveFile.ResourceTypes.Count; index++)
    //     {
    //         sortedResources.Add(archiveFile.ResourceTypes[index].Name, new Dictionary<int, string>());
    //         resourcePatchAvailable.Add(archiveFile.ResourceTypes[index].Name, new List<KeyValuePair<int, bool>>());
    //     }
    //
    //     for (var index = 0; index < archiveFile.ResourceEntries.Count; index++)
    //     {
    //         string type = archiveFile.ResourceTypes[archiveFile.ResourceEntries[index].TypeId].Name;
    //         string name = type == "Mipmap" ? 
    //             archiveFile.ResourceNames[index].Remove(0, 4) : 
    //             archiveFile.ResourceNames[index];
    //
    //         if (!sortedResources.ContainsKey(type))
    //         {
    //             continue;
    //         }
    //
    //         sortedResources[type].Add(index, name);
    //
    //         if (!patchFile.UnkInts1.Contains(index))
    //         {
    //             continue;
    //         }
    //
    //         resourcePatchAvailable[type].Add(new KeyValuePair<int, bool>(index, false));
    //     }
    //
    //     for (var index = 0; index < patchFile.Resources.Length; index++)
    //     {
    //         ResourceEntry entry = patchFile.Resources[index];
    //         string resourceTypeName = entry.TypeId < archiveFile.ResourceTypes.Count ? 
    //             archiveFile.ResourceTypes[entry.TypeId].Name : 
    //             "UnknownType";
    //         string saveName;
    //         string nameToPass = $"{resourceTypeName}_{index}";
    //
    //         if (resourcePatchAvailable.ContainsKey(resourceTypeName))
    //         {
    //             for (var z = 0; z < resourcePatchAvailable[resourceTypeName].Count; z++)
    //             {
    //                 var resourcePatch = resourcePatchAvailable[resourceTypeName][z];
    //                 
    //                 if (resourcePatch.Value)
    //                 {
    //                     continue;
    //                 }
    //
    //                 string storedName = sortedResources[resourceTypeName][resourcePatch.Key];
    //                         
    //                 if (!storedName.Equals("not available"))
    //                 {
    //                     nameToPass = storedName;
    //                 }
    //
    //                 resourcePatchAvailable[resourceTypeName][z] = new KeyValuePair<int, bool>(resourcePatch.Key, true);
    //                 break;
    //             }
    //         }
    //
    //         writer.WriteStartElement("ResourceEntry");
    //         writer.WriteElementString("Type", resourceTypeName);
    //         
    //         switch (resourceTypeName)
    //         {
    //             case "Texture":
    //                 bool isFileNameContainsDdsExtension = nameToPass.Contains(".dds");
    //                 nameToPass = !isFileNameContainsDdsExtension ? nameToPass + ".dds" : nameToPass; 
    //                 saveName = TextureEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Mipmap":
    //                 saveName = TextureMipMapEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "IndexBufferPool":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "VertexBufferPool":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "AnimalTrafficPaths":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "FrameResource":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Translokator":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Effects":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "FrameNameTable":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "EntityDataStorage":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "PREFAB":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "ItemDesc":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Actors":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Collisions":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Animation2":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "NAV_AIWORLD_DATA":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "NAV_OBJ_DATA":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "NAV_HPD_DATA":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "FxAnimSet":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "FxActor":
    //                 saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Sound":
    //                 nameToPass += ".fsb";
    //                 saveName = SoundEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             case "Script":
    //                 ScriptEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 continue;
    //             case "AudioSectors":
    //                 saveName = AudioSectorEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException($"Unknown resource type: {resourceTypeName}");
    //         }
    //         
    //         writer.WriteElementString("Version", entry.Version.ToString());
    //         string pathToWrite = Path.Join(pathToSave, saveName);
    //         Console.Out.WriteLine(pathToWrite); // TODO use logger and refactor all console log
    //         File.WriteAllBytes(pathToWrite, entry.Data!);
    //         writer.WriteEndElement();
    //     }
    //     
    //     writer.WriteEndElement();
    //     writer.Flush();
    //     writer.Dispose();
    // }

    public static ArchiveFile SerializeResources(string path)
    {
        string manifestFile = Path.Join(path, "manifest");

        if (!File.Exists(manifestFile))
        {
            throw new ArgumentException("Manifest file do not exists", nameof(path));
        }

        string manifestContent = File.ReadAllText(manifestFile);

        Manifest.Manifest manifest = Manifest.Manifest.Deserialize(manifestContent);
        
        return manifest.Version switch
        {
            19 => ResourceFunction19.SerializeResources(manifest, path),
            20 => ResourceFunction20.SerializeResources(manifest, path),
            _ => throw new ArgumentOutOfRangeException(
                nameof(manifest.Version), 
                $"Unknown version '{manifest.Version}'"
            )
        };
    }
}