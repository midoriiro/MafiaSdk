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

using System.Xml;
using System.Xml.XPath;
using Core.IO.FileFormats.Hashing;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class TableEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        TableResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = TableResource.Deserialize(entry.Version, stream, endian);
        }
        
        string pathToCreate = Path.Join(path, "tables");
        Directory.CreateDirectory(pathToCreate);

        writer.WriteElementString("NumTables", resource.Tables.Count.ToString());

        foreach (TableData data in resource.Tables)
        {
            //maybe we can get away with saving to version 1, and then converting to version 2 when packing?
            using (var stream = new MemoryStream())
            {
                data.Serialize(1, stream, endian);
                string pathToWrite = Path.Join(path, data.Name);
                File.WriteAllBytes(pathToWrite, stream.ToArray());
            }

            writer.WriteElementString("Table", data.Name);
        }

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
        
        var resource = new TableResource();

        //number of tables
        nodes.Current.MoveToNext();
        int count = nodes.Current.ValueAsInt;

        //read tables and add to resource.
        for (var i = 0; i != count; i++)
        {
            //goto next and read file name.
            nodes.Current.MoveToNext();
            string file = nodes.Current.Value;

            //create file data.
            var data = new TableData();

            //now read..
            string pathToRead = Path.Join(path, file);
            
            using (var reader = new BinaryReader(File.Open(pathToRead, FileMode.Open)))
            {
                TableData.Deserialize(1, reader.BaseStream, endian);
                data.Name = file;
                data.NameHash = FNV64.Hash(data.Name);
            }

            resource.Tables.Add(data);
        }

        //get version, always 1 Mafia II (2010) is 1, Mafia: DE (2020) is 2.
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);

        //create a temporary memory stream, merge all data and then fill entry data.
        using var stream = new MemoryStream();
        resource.Serialize(entry.Version, stream, endian);
        entry.Data = stream.ToArray();
        entry.SlotRamRequired = (uint)entry.Data.Length + 128;
        
        return entry;
    }
}