/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Text;
using System.Xml;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Utils;
using Core.IO.FileFormats.Streams;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS;

public class ArchiveFile
{
    public const uint Signature = 0x53445300; // 'SDS\0'

    public Endian Endian { get; private init; }
    public uint Version { get; private init; }
    public Platform Platform { get; private init; }
    public uint SlotRamRequired { get; private init; }
    public uint SlotVramRequired { get; private init; }
    public uint OtherRamRequired { get; private init; }
    public uint OtherVramRequired { get; private init; }
    public byte[]? Unknown20 { get; private init; }
    public string? ResourceInfoXml { get; private init; }
    public List<ResourceType> ResourceTypes { get; private init; } = null!;
    public List<ResourceEntry> ResourceEntries { get; private init; } = null!;
    public List<string> ResourceNames { get; private init; } = null!;
        
    private ArchiveFile()
    {
    }

    private static string ConstructResourceInfo(
        List<EntrySerializeResult> results, 
        IReadOnlyCollection<ResourceType> types
    )
    {
        var document = new XmlDocument();
            
        XmlElement root = document.CreateElement("xml");
        document.AppendChild(root);
            
        XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "utf-8", "yes");
        document.InsertBefore(declaration, document.DocumentElement);

        XmlAttribute ramAttribute = document.CreateAttribute("__type");
        ramAttribute.Value = "Int";

        foreach (EntrySerializeResult result in results)
        {
            XmlElement customDebugInfo = document.CreateElement("CustomDebugInfo");

            XmlElement typeName = document.CreateElement("TypeName");
            typeName.InnerText = types
                .Single(type => type.Id == result.ResourceEntry.TypeId)
                .Name;
                    
            XmlElement sourceDataDescription = document.CreateElement("SourceDataDescription");
            sourceDataDescription.InnerText = result.DataDescriptor;

            XmlElement slotRamRequired = document.CreateElement("SlotRamRequired");
            slotRamRequired.Attributes.Append((XmlAttribute)ramAttribute.Clone());
            slotRamRequired.InnerText = result.ResourceEntry.SlotRamRequired.ToString();
                
            XmlElement slotVramRequired = document.CreateElement("SlotVramRequired");
            slotVramRequired.Attributes.Append((XmlAttribute)ramAttribute.Clone());
            slotVramRequired.InnerText = result.ResourceEntry.SlotVramRequired.ToString();
                
            XmlElement otherRamRequired = document.CreateElement("OtherRamRequired");
            otherRamRequired.Attributes.Append((XmlAttribute)ramAttribute.Clone());
            otherRamRequired.InnerText = result.ResourceEntry.OtherRamRequired.ToString();
                
            XmlElement otherVramRequired = document.CreateElement("OtherVramRequired");
            otherVramRequired.Attributes.Append((XmlAttribute)ramAttribute.Clone());
            otherVramRequired.InnerText = result.ResourceEntry.OtherVramRequired.ToString();
                
            XmlElement resourceInfo = document.CreateElement("ResourceInfo");
            resourceInfo.AppendChild(customDebugInfo);
            resourceInfo.AppendChild(typeName);
            resourceInfo.AppendChild(sourceDataDescription);
            resourceInfo.AppendChild(slotRamRequired);
            resourceInfo.AppendChild(slotVramRequired);
            resourceInfo.AppendChild(otherRamRequired);
            resourceInfo.AppendChild(otherVramRequired);
            root.AppendChild(resourceInfo);
        }

        document.AppendChild(root);

        using var xmlWriter = new ResourceInfoXmlDocument();
        return xmlWriter.WriteDocument(document);
    }

    public static ArchiveFile From(Manifest manifest, List<EntrySerializeResult> results)
    {
        var slotRamRequired = (uint)results.Sum(result => result.ResourceEntry.SlotRamRequired);
        var slotVramRequired = (uint)results.Sum(result => result.ResourceEntry.SlotVramRequired);
        var otherRamRequired = (uint)results.Sum(result => result.ResourceEntry.OtherRamRequired);
        var otherVramRequired = (uint)results.Sum(result => result.ResourceEntry.OtherVramRequired);

        var resourceTypes = manifest.ResourceTypes;
        var resourceNames = manifest.Entries
            .Select(manifestEntry => manifestEntry.Descriptors.GetFilename())
            .Where(filename => filename is not null)
            .ToList();
        var resourceEntries = results
            .Select(result => result.ResourceEntry)
            .ToList();

        string? resourceInfo = null;

        if (manifest.RequireResourceInfoXml)
        {
            resourceInfo = ConstructResourceInfo(results, resourceTypes);   
        }

        return new ArchiveFile
        {
            Endian = manifest.Endian,
            Version = manifest.Version,
            Platform = manifest.Platform,
            SlotRamRequired = slotRamRequired,
            SlotVramRequired = slotVramRequired,
            OtherRamRequired = otherRamRequired,
            OtherVramRequired = otherVramRequired,
            Unknown20 = manifest.Unknown20,
            ResourceInfoXml = resourceInfo,
            ResourceTypes = resourceTypes,
            ResourceEntries = resourceEntries,
            ResourceNames = resourceNames!
        };
    }

    public void Serialize(Stream stream, ArchiveSerializeOptions options)
    {
        // If ratio isn't 0.0f, we'll try to compress.
        // If the user sets it to 0.0f, then we should skip any attempt to compress.
        bool compress = (options & ArchiveSerializeOptions.Compress) > 0;
        compress &= 0.9f != 0.0f; // TODO parametrize 0.9 compression ratio

        long basePosition = stream.Position;

        using (var dataStream = new MemoryStream(12))
        {
            dataStream.WriteValueU32(Signature, Endian.Big);
            dataStream.WriteValueU32(Version, Endian);
            dataStream.WriteValueU32((uint)Platform, Endian.Big);
            dataStream.Flush();
            stream.WriteFromMemoryStreamSafe(dataStream, Endian);
        }

        long headerPosition = stream.Position;
            
        stream.Seek(56, SeekOrigin.Current);
            
        var resourceTypeTableOffset = (uint)(stream.Position - basePosition);
            
        stream.WriteValueS32(ResourceTypes.Count, Endian);
            
        foreach (ResourceType resourceType in ResourceTypes)
        {
            resourceType.Write(stream, Endian);
        }
            
        var stride = (uint)(Version == 20 ? 38 : 30);
        var alignment = (uint)(Version == 20 ? 0x10000 : 0x4000);
        uint blockAlignment = (options & ArchiveSerializeOptions.OneBlock) != ArchiveSerializeOptions.None ? 
            (uint)ResourceEntries.Sum(entry => stride + (entry.Data?.Length ?? 0)) : 
            alignment;
        var blockTableOffset = (uint)(stream.Position - basePosition);

        BlockWriterStream blockStream = BlockWriterStream.ToStream(
            stream, 
            blockAlignment, 
            Endian, 
            compress, 
            false
        );           
            
        foreach (ResourceEntry resourceEntry in ResourceEntries)
        {
            var resourceHeader = new ResourceHeader
            {
                TypeId = (uint)resourceEntry.TypeId,
                Size = stride + (uint)(resourceEntry.Data?.Length ?? 0),
                Version = resourceEntry.Version,
                FileHash = resourceEntry.FileHash,
                SlotRamRequired = resourceEntry.SlotRamRequired,
                SlotVramRequired = resourceEntry.SlotVramRequired,
                OtherRamRequired = resourceEntry.OtherRamRequired,
                OtherVramRequired = resourceEntry.OtherVramRequired
            };

            using (var dataStream = new MemoryStream())
            {
                resourceHeader.Write(dataStream, Endian, Version);
                dataStream.Flush();
                blockStream.WriteFromMemoryStreamSafe(dataStream, Endian);
            }

            blockStream.WriteBytes(resourceEntry.Data!);
        }

        blockStream.Flush();
        blockStream.Finish();

        var xmlOffset = (uint)(stream.Position - basePosition);
            
        if (string.IsNullOrEmpty(ResourceInfoXml) == false)
        {
            stream.WriteString(ResourceInfoXml, Encoding.UTF8);
        }

        var fileHeader = new FileHeader
        {
            ResourceTypeTableOffset = resourceTypeTableOffset,
            BlockTableOffset = blockTableOffset,
            XmlOffset = xmlOffset,
            SlotRamRequired = SlotRamRequired,
            SlotVramRequired = SlotVramRequired,
            OtherRamRequired = OtherRamRequired,
            OtherVramRequired = OtherVramRequired,
            Flags = 1,
            Unknown20 = Unknown20 ?? new byte[16],
            ResourceCount = (uint)ResourceEntries.Count
        };

        stream.Position = headerPosition;
            
        using (var dataStream = new MemoryStream())
        {
            fileHeader.Write(dataStream, Endian);
            dataStream.Flush();
            stream.WriteFromMemoryStreamSafe(dataStream, Endian);
        }
    }

    private static ArchiveFile InternalDeserialize(Stream stream)
    {
        long basePosition = stream.Position;
            
        uint magic = stream.ReadValueU32(Endian.Big);
            
        if (magic != Signature)
        {
            throw new FormatException($"Unsupported Archive Signature: {magic}");
        }

        stream.Seek(8, SeekOrigin.Begin);
            
        // TODO check value for PS4 / Xbox One
        var platform = (Platform)stream.ReadValueU32(Endian.Big);
            
        if (platform != Platform.PC && platform != Platform.Xbox360 && platform != Platform.PS3)
        {
            throw new FormatException($"Unsupported Archive Platform: {platform}");
        }

        Endian endian = platform == Platform.PC ? Endian.Little : Endian.Big;

        stream.Seek(4, SeekOrigin.Begin);
            
        uint version = stream.ReadValueU32(endian);
            
        if (version != 19 && version != 20)
        {
            throw new FormatException($"Unsupported Archive Version: {version}");
        }

        stream.Seek(12, SeekOrigin.Begin);
        stream.Position = basePosition;

        using (MemoryStream dataStream = stream.ReadToMemoryStreamSafe(12, endian))
        {
            dataStream.Position += 4; // skip magic
            version = dataStream.ReadValueU32(endian);
            dataStream.Position += 4; // skip platform
        }

        if (version != 19 && version != 20)
        {
            throw new FormatException($"Unsupported Archive Version: {version}");
        }

        FileHeader fileHeader;
            
        using (MemoryStream dataStream = stream.ReadToMemoryStreamSafe(52, endian))
        {
            fileHeader = FileHeader.Read(dataStream, endian);
        }

        stream.Position = basePosition + fileHeader.ResourceTypeTableOffset;
        uint resourceTypeCount = stream.ReadValueU32(endian);
        var resourceTypes = new ResourceType[resourceTypeCount];
            
        for (uint index = 0; index < resourceTypeCount; index++)
        {
            resourceTypes[index] = ResourceType.Read(stream, endian);
        }

        stream.Position = basePosition + fileHeader.BlockTableOffset;
        BlockReaderStream blockStream = BlockReaderStream.FromStream(stream, endian);
        var resourceEntries = new ResourceEntry[fileHeader.ResourceCount];

        for (uint index = 0; index < fileHeader.ResourceCount; index++)
        {
            int size = version == 20 ? 34 : 26;
                
            ResourceHeader resourceHeader;
                
            using (MemoryStream dataStream = blockStream.ReadToMemoryStreamSafe(size, endian))
            {
                resourceHeader = ResourceHeader.Read(dataStream, endian, version);
            }

            if (resourceHeader.Size < 30)
            {
                throw new FormatException();
            }

            resourceEntries[index] = new ResourceEntry
            {
                TypeId = (int)resourceHeader.TypeId,
                Version = resourceHeader.Version,
                Data = blockStream.ReadBytes((int)resourceHeader.Size - (size + 4)),
                FileHash = resourceHeader.FileHash,
                SlotRamRequired = resourceHeader.SlotRamRequired,
                SlotVramRequired = resourceHeader.SlotVramRequired,
                OtherRamRequired = resourceHeader.OtherRamRequired,
                OtherVramRequired = resourceHeader.OtherVramRequired
            };
        }

        string? resourceInfoXml = null;
            
        // ReSharper disable once InvertIf
        if (fileHeader.XmlOffset != 0)
        {
            stream.Position = basePosition + fileHeader.XmlOffset;
            string xml = stream.ReadString((int)(stream.Length - stream.Position), Encoding.UTF8);
            resourceInfoXml = xml;
        }

        return new ArchiveFile
        {
            Version = version,
            Endian = endian,
            Platform = platform,
            SlotRamRequired = fileHeader.SlotRamRequired,
            SlotVramRequired = fileHeader.SlotVramRequired,
            OtherRamRequired = fileHeader.OtherRamRequired,
            OtherVramRequired = fileHeader.OtherVramRequired,
            Unknown20 = (byte[])fileHeader.Unknown20.Clone(),
            ResourceTypes = resourceTypes.ToList(),
            ResourceEntries = resourceEntries.ToList(),
            ResourceNames = new List<string>(),
            ResourceInfoXml = resourceInfoXml
        };
    }

    public static ArchiveFile Deserialize(Stream stream)
    {
        using Stream? archiveStream = ArchiveEncryption.Unwrap(stream);
        return InternalDeserialize(archiveStream ?? stream);
    }
}