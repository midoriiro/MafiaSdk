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

public class MemFileEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        MemFileResource resource;
        
        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = MemFileResource.Deserialize(entry.Version, stream, endian);
            entry.Data = resource.Data;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = resource.Name;
        }

        string pathToWrite = Path.Join(path, name);
        Directory.CreateDirectory(Path.GetDirectoryName(pathToWrite)!);
        
        writer.WriteElementString("File", name);
        writer.WriteElementString("Unk2_V4", resource.Unk2V4.ToString());
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
        
        //get file name from XML.
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value;
        nodes.Current.MoveToNext();
        var unk2 = Convert.ToUInt32(nodes.Current.Value);
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);

        //construct MemResource.
        var resource = new MemFileResource
        {
            Name = file,
            Unk1 = 1,
            Unk2V4 = unk2
        };

        // Read all the data, then allocate memory required
        string pathToRead = Path.Join(path, file);
        resource.Data = File.ReadAllBytes(pathToRead);
        entry.SlotRamRequired = (uint)resource.Data.Length;

        //serialize.
        using (var stream = new MemoryStream())
        {
            resource.Serialize(entry.Version, stream, Endian.Little);
            entry.Data = stream.ToArray();
        }

        sourceDataDescriptionNode.InnerText = file;
        return entry;
    }
}