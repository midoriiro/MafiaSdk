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
using Core.Games;
using Core.IO.FileFormats.Hashing;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class TextureEntry : IResourceEntry
{
    public static string Read(
        ResourceEntry entry,
        XmlWriter writer,
        string name,
        string path,
        Endian endian
    )
    {
        TextureResource resource;

        using (var stream = new MemoryStream(entry.Data!))
        {
            resource = TextureResource.Deserialize(entry.Version, stream, endian);
        }

        // TODO create global file names cache
        // if (_TextureNames.ContainsKey(resource.NameHash))
        // {
        //     string fetchedName = _TextureNames[resource.NameHash];
        //
        //     if (!string.IsNullOrEmpty(fetchedName))
        //     {
        //         name = fetchedName;
        //     }
        // }

        writer.WriteElementString("File", name);

        // We lack the file hash in M3 and M1: DE. So we have to add it to the file.
        if (GameWorkSpace.Instance().SelectedGame.Type is GamesEnumerator.Mafia1DefinitiveEdition or GamesEnumerator.Mafia3)
        {
            writer.WriteElementString("FileHash", resource.NameHash.ToString());
        }

        writer.WriteElementString("HasMIP", resource.HasMipMap.ToString());
        entry.Data = resource.Data;
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
        // We lack the file hash in M3 and M1: DE. So we have to get it from the file.
        bool isFusionGame = GameWorkSpace.Instance().SelectedGame.Type is GamesEnumerator.Mafia1DefinitiveEdition or GamesEnumerator.Mafia3;

        if (nodes.Current is null)
        {
            throw new NullReferenceException("Current node from node iterator is null");
        }

        //read from xml.
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value;
        nodes.Current.MoveToNext();

        ulong hash = 0;
        
        if (isFusionGame)
        {
            var hashString = nodes.Current.ToString();
            hash = Convert.ToUInt64(hashString);
            nodes.Current.MoveToNext();
        }

        var hasMip = Convert.ToByte(nodes.Current.Value);
        nodes.Current.MoveToNext();

        // Setup ResourceEntry MetaInfo.
        entry.Version = Convert.ToUInt16(nodes.Current.Value);
        sourceDataDescriptionNode.InnerText = file;

        // Begin serialising the Texture Resource.

        // Create stream.
        using var stream = new MemoryStream();
        // Read the Texture file (.DDS).
        string pathToRead = Path.Join(path, file);
        byte[] data = File.ReadAllBytes(pathToRead);
        var resource = new TextureResource(FNV64.Hash(file), hasMip, data);

        // Do Version specific handling;
        if (isFusionGame)
        {
            resource.NameHash = hash;
        }

        // Serialise to the TextureResource and pack it into the ResourceEntry.
        resource.Serialize(entry.Version, stream, endian);
        entry.Data = stream.ToArray();

        // Configure VRAM information for the SDS.
        if (GameWorkSpace.Instance().SelectedGame.Type is GamesEnumerator.Mafia2 or GamesEnumerator.Mafia2DefinitiveEdition)
        {
            entry.SlotVramRequired = (uint)(data.Length - 128);
            //if (hasMIP == 1)
            //{
            //    using (BinaryReader reader = new BinaryReader(File.Open(sdsFolder + "/MIP_" + file, FileMode.Open)))
            //        entry.SlotVramRequired += (uint)(reader.BaseStream.Length - 128);
            //}
        }

        if (GameWorkSpace.Instance().SelectedGame.Type != GamesEnumerator.Mafia1DefinitiveEdition)
        {
            return entry;
        }

        int size = (resource.IsDirectX10 ? 157 : 137);
        entry.SlotVramRequired = (uint)(stream.Length - size);

        return entry;
    }
}