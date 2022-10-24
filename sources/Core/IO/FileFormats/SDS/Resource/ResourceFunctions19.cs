// using System.Xml;
// using System.Xml.XPath;
// using Core.IO.FileFormats.SDS.Archive;
// using Core.IO.ResourceFormats;
//
// namespace Core.IO.FileFormats.SDS.Resource;
//
// public static class ResourceFunction19
// {
//     public static void SaveResources(XmlWriter writer, ArchiveFile archiveFile)
//     {
//         var counts = new int[archiveFile.ResourceTypes.Count];
//
//         //TODO Cleanup this code. It's awful. (V2 26/08/18, improved to use switch)
//         for (var index = 0; index != archiveFile.ResourceEntries.Count; index++)
//         {
//             ResourceEntry entry = archiveFile.ResourceEntries[index];
//
//             if (entry.TypeId == -1)
//             {
//                 // TODO log this
//                 // MessageBox.Show(string.Format("Detected unknown type, skipping. Size: {0}", entry.Data.Length),
//                 // "Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                 continue;
//             }
//
//             writer.WriteStartElement("ResourceEntry");
//             writer.WriteElementString("Type", archiveFile.ResourceTypes[entry.TypeId].Name);
//             
//             var saveName = "";
//             string resourceName = archiveFile.ResourceNames[index];
//
//             // TODO log this
//             // Log.WriteLine("Resource: " + i + ", name: " + itemNames[i] + ", type: " + entry.TypeId);
//
//             string sdsToolName = archiveFile.ResourceTypes[entry.TypeId].Name + "_" + counts[entry.TypeId] + ".bin";
//             
//             switch (archiveFile.ResourceTypes[entry.TypeId].Name)
//             {
//                 case "Texture":
//                     TextureResource.ReadXmlEntry(entry, writer, resourceName, null!, archiveFile.Endian);
//                     saveName = resourceName;
//                     break;
//                 case "Mipmap":
//                     ReadMipmapEntry(entry, writer, resourceName);
//                     saveName = "MIP_" + resourceName;
//                     break;
//                 case "IndexBufferPool":
//                     saveName = ReadBasicEntry(writer,
//                         ToolkitSettings.UseSDSToolFormat == false ? "IndexBufferPool_" + index + ".ibp" : sdsToolName);
//                     break;
//                 case "VertexBufferPool":
//                     saveName = ReadBasicEntry(writer,
//                         ToolkitSettings.UseSDSToolFormat == false ? "VertexBufferPool_" + index + ".vbp" : sdsToolName);
//                     break;
//                 case "AnimalTrafficPaths":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "AnimalTrafficPaths" + index + ".atp" : sdsToolName);
//                     break;
//                 case "FrameResource":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "FrameResource_" + index + ".fr" : sdsToolName);
//                     break;
//                 case "Effects":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Effects_" + index + ".eff" : sdsToolName);
//                     break;
//                 case "FrameNameTable":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "FrameNameTable_" + index + ".fnt" : sdsToolName);
//                     break;
//                 case "EntityDataStorage":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "EntityDataStorage_" + index + ".eds" : sdsToolName);
//                     break;
//                 case "PREFAB":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "PREFAB_" + index + ".prf" : sdsToolName);
//                     break;
//                 case "ItemDesc":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "ItemDesc_" + index + ".ids" : sdsToolName);
//                     break;
//                 case "Actors":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Actors_" + index + ".act" : sdsToolName);
//                     break;
//                 case "Collisions":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Collisions_" + index + ".col" : sdsToolName);
//                     break;
//                 case "AudioSectors":
//                     ReadAudioSectorEntry(entry, resourceXml, resourceName, finalPath);
//                     saveName = resourceName;
//                     break;
//                 case "SoundTable":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "SoundTable_" + index + ".stbl" : sdsToolName);
//                     break;
//                 case "Speech":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Speech_" + index + ".spe" : sdsToolName);
//                     break;
//                 case "FxAnimSet":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "FxAnimSet_" + index + ".fas" : sdsToolName);
//                     break;
//                 case "FxActor":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "FxActor_" + index + ".fxa" : sdsToolName);
//                     break;
//                 case "Cutscene":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Cutscene_" + index + ".cut" : sdsToolName);
//                     break;
//                 case "Translokator":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "Translokator_" + index + ".tra" : sdsToolName);
//                     break;
//                 case "Animation2":
//                     saveName = ReadBasicEntry(resourceXml, resourceName + ".an2");
//                     break;
//                 case "NAV_AIWORLD_DATA":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "NAV_AIWORLD_DATA_" + index + ".nav" : sdsToolName);
//                     break;
//                 case "NAV_OBJ_DATA":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "NAV_OBJ_DATA_" + index + ".nov" : sdsToolName);
//                     break;
//                 case "NAV_HPD_DATA":
//                     saveName = ReadBasicEntry(resourceXml,
//                         ToolkitSettings.UseSDSToolFormat == false ? "NAV_HPD_DATA_" + index + ".nhv" : sdsToolName);
//                     break;
//                 case "Script":
//                     ScriptResource.ReadXmlEntry(entry, writer, null!, finalPath, archiveFile.Endian);
//                     continue;
//                 case "XML":
//                     XmlResource.ReadXmlEntry(entry, writer, resourceName, finalPath, archiveFile.Endian);
//                     continue;
//                 case "Sound":
//                     ReadSoundEntry(entry, resourceXml, resourceName, finalPath);
//                     saveName = resourceName + ".fsb";
//                     break;
//                 case "MemFile":
//                     MemFileResource.ReadXmlEntry(entry, writer, resourceName, finalPath, archiveFile.Endian);
//                     saveName = resourceName;
//                     break;
//                 case "Table":
//                     TableResource.ReadXmlEntry(entry, writer, null!, finalPath, archiveFile.Endian);
//                     counts[archiveFile.ResourceTypes[entry.TypeId].Id]++;
//                     writer.WriteElementString("Version", entry.Version.ToString());
//                     writer.WriteEndElement();
//                     continue;
//                 case "Animated Texture":
//                     saveName = ReadBasicEntry(resourceXml, resourceName);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(archiveFile.ResourceTypes[entry.TypeId].Name);
//                     break;
//             }
//
//             counts[archiveFile.ResourceTypes[entry.TypeId].Id]++;
//             writer.WriteElementString("Version", entry.Version.ToString());
//             File.WriteAllBytes(finalPath + "/" + saveName, entry.Data!);
//             writer.WriteEndElement();
//         }
//
//         writer.WriteEndElement();
//         writer.Flush();
//         writer.Dispose();
//     }
//
//     public static bool BuildResources(
//         XmlDocument document,
//         XmlDocument xmlDoc,
//         XmlNode rootNode,
//         string sdsFolder,
//         ArchiveFile archiveFile
//     )
//     {
//         XPathNavigator nav = document.CreateNavigator()!;
//         XPathNodeIterator nodes = nav.Select("/SDSResource/ResourceEntry");
//         var entries = new Dictionary<string, List<ResourceEntry>>();
//
//         while (nodes.MoveNext())
//         {
//             if (nodes.Current is null)
//             {
//                 continue;
//             }
//
//             nodes.Current.MoveToFirstChild();
//             string resourceType = nodes.Current.Value;
//
//             if (!entries.ContainsKey(resourceType))
//             {
//                 var resource = new ResourceType
//                 {
//                     Name = nodes.Current.Value,
//                     Id = (uint)entries.Count
//                 };
//
//                 //TODO
//                 if (resource.Name == "IndexBufferPool" || resource.Name == "PREFAB")
//                     resource.Parent = 3;
//                 else if (resource.Name == "VertexBufferPool" || resource.Name == "NAV_OBJ_DATA")
//                     resource.Parent = 2;
//                 else if (resource.Name == "NAV_HPD_DATA")
//                     resource.Parent = 1;
//
//                 archiveFile.ResourceTypes.Add(resource);
//                 entries.Add(resourceType, new List<ResourceEntry>());
//             }
//
//             XmlNode resourceNode = xmlDoc.CreateElement("ResourceInfo");
//             XmlNode typeNameNode = xmlDoc.CreateElement("TypeName");
//             typeNameNode.InnerText = resourceType;
//             XmlNode sourceDataDescriptionNode = xmlDoc.CreateElement("SourceDataDescription");
//
//             var resourceEntry = new ResourceEntry();
//
//             switch (resourceType)
//             {
//                 case "FrameResource":
//                 case "Effects":
//                 case "PREFAB":
//                 case "ItemDesc":
//                 case "FrameNameTable":
//                 case "Actors":
//                 case "NAV_AIWORLD_DATA":
//                 case "NAV_OBJ_DATA":
//                 case "NAV_HPD_DATA":
//                 case "Cutscene":
//                 case "FxActor":
//                 case "FxAnimSet":
//                 case "Translokator":
//                 case "Speech":
//                 case "SoundTable":
//                 case "AnimalTrafficPaths":
//                     resourceEntry = WriteBasicEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "AudioSectors":
//                     resourceEntry = WriteAudioSectorEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Animated Texture":
//                     resourceEntry =
//                         WriteAnimatedTextureEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Collisions":
//                     resourceEntry = WriteCollisionEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "IndexBufferPool":
//                 case "VertexBufferPool":
//                     resourceEntry = WriteBufferEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "EntityDataStorage":
//                     resourceEntry = WriteEntityDataEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Animation2":
//                     resourceEntry = WriteAnimationEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Texture":
//                     resourceEntry = WriteTextureEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Mipmap":
//                     resourceEntry = WriteMipmapEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Sound":
//                     resourceEntry = WriteSoundEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "XML":
//                     resourceEntry = WriteXMLEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "MemFile":
//                     resourceEntry = WriteMemFileEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Script":
//                     resourceEntry = WriteScriptEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 case "Table":
//                     resourceEntry = WriteTableEntry(resourceEntry, nodes, sdsFolder, sourceDataDescriptionNode);
//                     break;
//                 default:
//                     MessageBox.Show("Did not pack type: " + resourceType, "Toolkit", MessageBoxButtons.OK,
//                         MessageBoxIcon.Information);
//                     break;
//             }
//
//             resourceNode.AppendChild(typeNameNode);
//             resourceNode.AppendChild(sourceDataDescriptionNode);
//             resourceNode.AppendChild(AddRamElement(xmlDoc, "SlotRamRequired", (int)resourceEntry.SlotRamRequired));
//             resourceNode.AppendChild(AddRamElement(xmlDoc, "SlotVRamRequired", (int)resourceEntry.SlotVramRequired));
//             resourceNode.AppendChild(AddRamElement(xmlDoc, "OtherRamRequired", (int)resourceEntry.OtherRamRequired));
//             resourceNode.AppendChild(AddRamElement(xmlDoc, "OtherVramRequired", (int)resourceEntry.OtherVramRequired));
//             rootNode.AppendChild(resourceNode);
//             archiveFile.SlotRamRequired += resourceEntry.SlotRamRequired;
//             archiveFile.SlotVramRequired += resourceEntry.SlotVramRequired;
//             archiveFile.OtherRamRequired += resourceEntry.OtherRamRequired;
//             archiveFile.OtherVramRequired += resourceEntry.OtherVramRequired;
//
//             // Find TypeId then add to the big list of resources
//             resourceEntry.TypeId = (int)archiveFile.ResourceTypes.Find(s => s.Name.Equals(resourceType)).Id;
//             archiveFile.ResourceEntries.Add(resourceEntry);
//         }
//
//         // Update Meta-Info or 'Outer-XML'
//         archiveFile.ResourceInfoXml = xmlDoc.OuterXml;
//
//         return true;
//     }
// }