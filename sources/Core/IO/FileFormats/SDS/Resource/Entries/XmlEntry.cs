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

//THIS VERSION IS MODIFIED.
//SEE ORIGINAL CODE HERE::
//https://github.com/gibbed/Gibbed.Illusion

using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class XmlEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path,
        Endian endian)
    {
        XmlResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            // Unpack our XML Resource.
            resource = XmlResource.Deserialize(entry.Version, stream, endian);
            name = resource.Name;

            // Create the directories.
            string pathToCreate = Path.Join(path, name);
            Directory.CreateDirectory(Path.GetDirectoryName(pathToCreate)!);

            // Set the filename we want to save the file too.
            string fileName = Path.Join(path, Path.GetFileName(name) + ".xml");

            if (resource.HasFailedToDecompile)
            {
                byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));
                File.WriteAllBytes(fileName, data);
            }
            else
            {
                // 08/08/2020. Originally was File.WriteAllText, but caused problems with some XML documents.
                using var streamWriter = new StreamWriter(File.Open(fileName, FileMode.Create));
                streamWriter.WriteLine(resource.Content);
            }
        }

        writer.WriteElementString("File", name);
        writer.WriteElementString("XMLTag", resource.Tag);
        writer.WriteElementString("Unk1", Convert.ToByte(resource.Unk1).ToString());
        writer.WriteElementString("Unk3", Convert.ToByte(resource.Unk3).ToString());
        writer.WriteElementString("FailedToDecompile", Convert.ToByte(resource.HasFailedToDecompile).ToString());
        writer.WriteElementString("Version", entry.Version.ToString());
        writer.WriteEndElement(); //finish early.
        return name;
    }

    public static ResourceEntry Write(
        ResourceEntry entry, 
        XPathNodeIterator nodes,
        XmlNode sourceDataDescriptionNode,
        string path, 
        Endian endian
    )
    {
        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }
        
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value;
        sourceDataDescriptionNode.InnerText = file;

        nodes.Current.MoveToNext();
        string tag = nodes.Current.Value;

        nodes.Current.MoveToNext();
        bool unk1 = nodes.Current.ValueAsBoolean;

        nodes.Current.MoveToNext();
        bool unk3 = nodes.Current.ValueAsBoolean;

        nodes.Current.MoveToNext();
        bool failedToDecompile = nodes.Current.ValueAsBoolean;

        //need to do version early.
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);

        using var stream = new MemoryStream();

        var resource = new XmlResource
        {
            Name = file,
            Content = Path.Join(path, file + ".xml"),
            Tag = tag,
            Unk1 = unk1,
            Unk3 = unk3,
            HasFailedToDecompile = failedToDecompile
        };

        resource.Serialize(entry.Version, stream, endian);

        if (resource.Unk3)
        {
            entry.Data = stream.ToArray();
        }
        else
        {
            entry.Data = stream.ToArray();
        }

        return entry;
    }
}