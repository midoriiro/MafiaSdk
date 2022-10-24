using System.Text;
using System.Xml;
using System.Xml.XPath;
using Core.Games;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource;

public static class Resource
{
    private static readonly Dictionary<string, string> _fileExtensionLookup = new()
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
        { "Animated Texture", ".ifl" },
    };

    private static readonly Dictionary<string, string> _fileExtensionLookupFusion = new()
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

        for (var i = 0; i != archiveFile.ResourceTypes.Count; i++)
        {
            // check if resource type has empty name
            if (archiveFile.ResourceTypes[i].Name == "")
            {
                crySdsType = (int)archiveFile.ResourceTypes[i].Id;
            }
        }


        if (crySdsType == -1)
        {
            return null;
        }


        for (var i = 0; i < archiveFile.ResourceEntries.Count; i++)
        {
            if (archiveFile.ResourceEntries[i].TypeId != crySdsType)
            {
                continue;
            }

            // Fix for CrySDS archives
            using var stream = new MemoryStream(archiveFile.ResourceEntries[i].Data!);

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
            archiveFile.ResourceEntries.RemoveAt(i);
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
            extension = _fileExtensionLookup[typename];
            return extension;
        }

        extension = _fileExtensionLookupFusion[typename];
        return extension;
    }

    public static void SaveResources(FileInfo file, ArchiveFile archiveFile, GamesEnumerator selectedGame)
    {
        XPathDocument? document = null;

        // pull XML from resource info XML
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

        // stub out file names
        for (var i = 0; i < archiveFile.ResourceEntries.Count; i++)
        {
            ResourceEntry entry = archiveFile.ResourceEntries[i];

            var fileName = "unknown_0";

            if (entry.TypeId != -1)
            {
                // TODO: Determine if this could be done for fusion games
                var nameOfFile = "file";

                if (selectedGame is GamesEnumerator.Mafia2 or GamesEnumerator.Mafia2DefinitiveEdition)
                {
                    nameOfFile = archiveFile.ResourceTypes[entry.TypeId].Name;
                }

                // Get extension, format filename properly.
                string extension = DetermineFileExtension(archiveFile.ResourceTypes[entry.TypeId].Name, selectedGame);
                fileName = $"{nameOfFile}_{i}{extension}";
            }

            archiveFile.ResourceNames.Add(fileName);
        }

        // Pull names from XML
        if (document is not null)
        {
            XPathNavigator nav = document.CreateNavigator();
            XPathNodeIterator nodes = nav.Select("/xml/ResourceInfo/SourceDataDescription");

            // iterate and update name
            var index = 0;

            while (nodes.MoveNext())
            {
                if (nodes.Current is null)
                {
                    continue;
                }

                string name = nodes.Current.Value;

                if (!name.Equals("not available"))
                {
                    archiveFile.ResourceNames[index] = name;
                }

                index++;
            }

            // TODO log this
            // Log.WriteLine("Found all items; count is " + nodes.Count);
        }
        
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = ("\t"),
            OmitXmlDeclaration = true
        };

        // Create extraction directory
        string pathToSave = file.FullName;
        Directory.CreateDirectory(pathToSave);

        // TODO log this
        // Log.WriteLine("Begin unpacking and saving files..");

        // Save resource
        string contentPath = Path.Join(pathToSave, "SDSContent.xml");

        var writer = XmlWriter.Create(contentPath, settings);
        writer.WriteStartElement("SDSResource");

        switch (archiveFile.Version)
        {
            // case 19:
            //     ResourceFunction19.SaveResources(writer, archiveFile);
            //     break;
            case 20:
                ResourceFunction20.SaveResources(writer, archiveFile, pathToSave);
                break;
        }
    }

    // public bool BuildResources(string folder, ArchiveFile archiveFile)
    // {
    //     string sdsFolder = folder;
    //     XmlDocument? sdsDocument;
    //
    //     string sdsContentPath = Path.Join(sdsFolder, "SDSContent.xml");
    //
    //     // First check if it actually exists
    //     if (!File.Exists(sdsContentPath))
    //     {
    //         // TODO log this
    //         // MessageBox.Show("SDSContent.xml does not exist. Cannot pack this SDS.", "Mafia Toolkit");
    //         return false;
    //     }
    //
    //     // Then attempt to read
    //     // Attempt to sort the file.
    //     // Only works for M2 and M2DE.
    //     if (ChosenGameType == GamesEnumerator.MafiaII || ChosenGameType == GamesEnumerator.MafiaII_DE)
    //     {
    //         // Loading then saving automatically sorts.
    //         SDSContentFile sdsContent = new SDSContentFile();
    //         sdsContent.ReadFromFile(new FileInfo(sdsContentPath));
    //         sdsContent.WriteToFile();
    //     }
    //
    //     // Open a FileStream which contains the SDSContent data.
    //     using (var xmlStream = new FileStream(sdsContentPath, FileMode.Open))
    //     {
    //         try
    //         {
    //             sdsDocument = new XmlDocument();
    //             sdsDocument.Load(xmlStream);
    //         }
    //         catch (Exception exception)
    //         {
    //             // TODO log this
    //             // MessageBox.Show($"Error while parsing SDSContent.XML. \n{ex.Message}");
    //             return false;
    //         }
    //     }
    //
    //     // GoAhead and begin creating the document to save inside the SDSContent.
    //     var document = new XmlDocument();
    //     XmlNode rootNode = document.CreateElement("xml");
    //     document.AppendChild(rootNode);
    //
    //     // Try and pack the resources found in SDSContent.
    //     return archiveFile.Version switch
    //     {
    //         19 => ResourceFunction19.BuildResources(sdsDocument, document, rootNode, sdsFolder, archiveFile),
    //         20 => ResourceFunction20.BuildResources(sdsDocument, document, rootNode, sdsFolder, archiveFile),
    //         _ => false
    //     };
    // }
}