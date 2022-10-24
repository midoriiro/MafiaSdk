using System.Xml;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Caches;
using Core.IO.FileFormats.SDS.Resource.Entries;

namespace Core.IO.FileFormats.SDS.Resource;

public static class ResourceFunction20
{
    private static readonly ResourceNameCache FileNamesAndHash = ResourceNameCache.FromFile(
        "Resources/Caches/M3DE_ResourceNameDatabase.txt"    
    );
    
    public static void SaveResources(
        XmlWriter writer, 
        ArchiveFile archiveFile,
        string pathToSave
    )
    {
        //TODO Cleanup this code. It's awful. (V2 26/08/18, improved to use switch)
        for (var i = 0; i != archiveFile.ResourceEntries.Count; i++)
        {
            ResourceEntry entry = archiveFile.ResourceEntries[i];
            
            if (entry.TypeId == -1)
            {
                // TODO log this
                // MessageBox.Show(string.Format("Detected unknown type, skipping. Size: {0}", entry.Data.Length),
                    // "Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                continue;
            }

            string saveName;
            string nameToPass = archiveFile.ResourceNames[i];
            string resourceTypeName = archiveFile.ResourceTypes[entry.TypeId].Name;
            string? fileName = FileNamesAndHash.HasFilename(entry);
            
            if (!string.IsNullOrEmpty(fileName))
            {
                string nameToPassWithoutExtension = Path.GetFileNameWithoutExtension(nameToPass);
                
                if (fileName.Contains('*') || fileName.Contains('+'))
                {
                    fileName = fileName.Replace("*", nameToPassWithoutExtension);
                    fileName = fileName.Replace("+", nameToPassWithoutExtension);
                }
                
                nameToPass = fileName;
            }

            writer.WriteStartElement("ResourceEntry");
            writer.WriteAttributeString("FileGUID", entry.FileHash.ToString());
            writer.WriteElementString("Type", resourceTypeName);

            switch (resourceTypeName)
            {
                case "Texture":
                    saveName = TextureEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    break;
                case "hkAnimation":
                    saveName = HavokEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    break;
                case "NAV_PATH_DATA":
                case "NAV_AIWORLD_DATA":
                case "RoadMap":
                case "EnlightenResource":
                    saveName = BasicEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    break;
                case "Generic":
                    GenericEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    continue;
                case "MemFile":
                    nameToPass = (archiveFile.ResourceInfoXml is null ? string.Empty : nameToPass);
                    saveName = MemFileEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    break;
                case "SystemObjectDatabase":
                    XBinEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    continue;
                case "XML":
                    nameToPass = (archiveFile.ResourceInfoXml is null ? string.Empty : nameToPass);
                    XmlEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    continue;
                case "Flash":
                    nameToPass = (archiveFile.ResourceInfoXml is null ? string.Empty : nameToPass);
                    saveName = FlashEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    break;
                case "Script":
                    ScriptEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    continue;
                case "Cutscene":
                    CutsceneEntry.Read(entry, writer, nameToPass, pathToSave, archiveFile.Endian);
                    continue;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown resource type: {resourceTypeName}");
            }

            writer.WriteElementString("Version", entry.Version.ToString());
            string pathToWrite = Path.Join(pathToSave, saveName);
            Console.Out.WriteLine(pathToWrite); // TODO use logger and refactor all console log
            File.WriteAllBytes(pathToWrite, entry.Data!);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.Close();
        writer.Flush();
        writer.Dispose();
    }

    // public static bool BuildResources(
    //     XmlDocument document, 
    //     XmlDocument xmlDoc, 
    //     XmlNode rootNode, 
    //     string sdsFolder, 
    //     ArchiveFile archiveFile
    // )
    // {
    //     XPathNavigator nav = document.CreateNavigator()!;
    //     XPathNodeIterator nodes = nav.Select("/SDSResource/ResourceEntry");
    //     var entries = new Dictionary<string, List<ResourceEntry>>();
    //     
    //     while (nodes.MoveNext())
    //     {
    //         if (nodes.Current is null)
    //         {
    //             continue;
    //         }
    //         
    //         // Get file guid
    //         nodes.Current.MoveToFirstAttribute();
    //         ulong inFileHash = ulong.Parse(nodes.Current.Value);
    //
    //         // move to the child nodes of this element
    //         nodes.Current.MoveToParent();
    //         nodes.Current.MoveToFirstChild();
    //         string resourceType = nodes.Current.Value;
    //
    //         // see if we need to create new type
    //         if (!entries.ContainsKey(resourceType))
    //         {
    //             var resource = new ResourceType
    //             {
    //                 Name = nodes.Current.Value,
    //                 Id = (uint)entries.Count
    //             };
    //
    //             //TODO
    //             if (resource.Name == "NAV_PATH_DATA")
    //             {
    //                 resource.Parent = 1;
    //             }
    //
    //             archiveFile.ResourceTypes.Add(resource);
    //             entries.Add(resourceType, new List<ResourceEntry>());
    //         }
    //
    //         // begin creating xml for saving
    //         XmlNode resourceNode = xmlDoc.CreateElement("ResourceInfo");
    //         XmlNode typeNameNode = xmlDoc.CreateElement("TypeName");
    //         typeNameNode.InnerText = resourceType;
    //         XmlNode sourceDataDescriptionNode = xmlDoc.CreateElement("SourceDataDescription");
    //
    //         var resourceEntry = new ResourceEntry
    //         {
    //             FileHash = inFileHash
    //         };
    //
    //         switch (resourceType)
    //         {
    //             case "Texture":
    //                 resourceEntry = WriteTextureEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "hkAnimation":
    //                 resourceEntry = WriteHavokEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "Generic":
    //                 resourceEntry = WriteGenericEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 resourceEntry.FileHash = inFileHash;
    //                 break;
    //             case "NAV_PATH_DATA":
    //             case "NAV_AIWORLD_DATA":
    //             case "RoadMap":
    //             case "EnlightenResource":
    //                 resourceEntry = WriteBasicEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 resourceEntry.SlotRamRequired += (uint)(resourceEntry.Data.Length);
    //                 break;
    //             case "MemFile":
    //                 resourceEntry = WriteMemFileEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "SystemObjectDatabase":
    //                 resourceEntry = WriteXBinEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "XML":
    //                 resourceEntry = WriteXMLEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "Script":
    //                 resourceEntry = WriteScriptEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "Flash":
    //                 resourceEntry = WriteFlashEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             case "Cutscene":
    //                 resourceEntry = WriteCutsceneEntry(resourceEntry, nodes, sdsFolder, sddescNode);
    //                 break;
    //             default:
    //                 MessageBox.Show("Did not pack type: " + resourceType, "Toolkit", MessageBoxButtons.OK,
    //                     MessageBoxIcon.Information);
    //                 break;
    //         }
    //
    //         resourceNode.AppendChild(typeNameNode);
    //         resourceNode.AppendChild(sourceDataDescriptionNode);
    //         resourceNode.AppendChild(AddRamElement(xmlDoc, "SlotRamRequired", (int)resourceEntry.SlotRamRequired));
    //         resourceNode.AppendChild(AddRamElement(xmlDoc, "SlotVRamRequired", (int)resourceEntry.SlotVramRequired));
    //         resourceNode.AppendChild(AddRamElement(xmlDoc, "OtherRamRequired", (int)resourceEntry.OtherRamRequired));
    //         resourceNode.AppendChild(AddRamElement(xmlDoc, "OtherVramRequired", (int)resourceEntry.OtherVramRequired));
    //         rootNode.AppendChild(resourceNode);
    //
    //         archiveFile.SlotVramRequired += resourceEntry.SlotVramRequired;
    //         archiveFile.SlotRamRequired += resourceEntry.SlotRamRequired;
    //         archiveFile.OtherRamRequired += resourceEntry.OtherRamRequired;
    //
    //         resourceEntry.TypeId = (int)archiveFile.ResourceTypes.Find(s => s.Name.Equals(resourceType)).Id;
    //         entries[resourceType].Add(resourceEntry);
    //     }
    //
    //     archiveFile.ResourceTypes.Reverse();
    //
    //     for (var i = 0; i < archiveFile.ResourceTypes.Count; i++)
    //     {
    //         ResourceType entry = archiveFile.ResourceTypes[i];
    //         entry.Id = (uint)i;
    //         archiveFile.ResourceTypes[i] = entry;
    //     }
    //
    //     foreach ((string? key, var value) in entries)
    //     {
    //         foreach (ResourceEntry entry in value)
    //         {
    //             entry.TypeId = (int)archiveFile.ResourceTypes.Find(s => s.Name.Equals(key)).Id;
    //         }
    //     }
    //
    //     foreach (var pair in entries)
    //     {
    //         archiveFile.ResourceEntries.AddRange(pair.Value);
    //     }
    //
    //     archiveFile.ResourceInfoXml = xmlDoc.OuterXml;
    //     return true;
    // }
}