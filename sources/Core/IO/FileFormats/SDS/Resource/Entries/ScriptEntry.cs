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
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class ScriptEntry : IResourceEntry
{
    public static bool DecompileScript { get; set; } = false;
    
    public static string Read(
        ResourceEntry entry, 
        XmlWriter writer, 
        string name, 
        string path, 
        Endian endian
    )
    {
        ScriptResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = ScriptResource.Deserialize(entry.Version, stream, endian);
        }

        writer.WriteElementString("File", resource.Path);
        writer.WriteElementString("ScriptNum", resource.Scripts.Count.ToString());

        for (var x = 0; x != resource.Scripts.Count; x++)
        {
            // Get the script resource.
            ScriptData scriptItem = resource.Scripts[x];

            // Get directory and Script name.
            string scriptDirectory = Path.GetDirectoryName(scriptItem.Name)!;
            string scriptName = Path.GetFileName(scriptItem.Name);

            // Create the new directory.
            string newDirectory = Path.Join(path, scriptDirectory);
            Directory.CreateDirectory(newDirectory);

            // Write the script data to the designated file.
            string scriptPath = Path.Join(newDirectory, scriptName);
            File.WriteAllBytes(scriptPath, scriptItem.Data);

            // If user requests, decompile the Lua file.
            if (DecompileScript)
            {                 
                var info = new FileInfo(scriptPath);
                // TODO create lua decompiler
                // FileLua luaFile = new FileLua(info);
                // luaFile.TryDecompileBytecode();
            }

            writer.WriteElementString("Name", scriptItem.Name);
        }
        
        writer.WriteElementString("Version", entry.Version.ToString());
        writer.WriteEndElement(); // We finish early with scripts, as this has an alternate layout.

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
        
        // Get data from Xml.
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value;
        nodes.Current.MoveToNext();
        var numScripts = Convert.ToInt32(nodes.Current.Value);

        // Create the new resource, add file.
        var resource = new ScriptResource
        {
            Path = file
        };

        // Iterate through scripts, reading each one and pushing them into the list.
        for (var i = 0; i < numScripts; i++)
        {
            var data = new ScriptData();
            nodes.Current.MoveToNext();
            data.Name = nodes.Current.Value;
            string pathToRead = Path.Join(path, data.Name);
            data.Data = File.ReadAllBytes(pathToRead);
            resource.Scripts.Add(data);
        }

        // Finish reading the Xml by getting the version.
        nodes.Current.MoveToNext();
        var version = Convert.ToUInt16(nodes.Current.Value);

        // Create the stream and serialize the resource package into said stream.
        using(var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            entry.Data = stream.ToArray();

            entry.SlotRamRequired = resource.GetRawBytes();
        }

        // Set the entry version and setup the data for the meta info.
        entry.Version = version;        
        sourceDataDescriptionNode.InnerText = "Not Available";
        return entry;
    }
}