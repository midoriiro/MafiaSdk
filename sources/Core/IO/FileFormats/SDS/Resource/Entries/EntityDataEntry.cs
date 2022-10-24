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
using Core.IO.FileFormats.Hashing;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class EntityDataEntry : IResourceEntry
{
    public static string? Read(
        ResourceEntry entry,
        XmlWriter writer,
        string name,
        string path,
        Endian endian
    )
    {
        throw new NotImplementedException();
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
        
        //get data from XML
        nodes.Current.MoveToNext();
        string file = nodes.Current.Value;
        nodes.Current.MoveToNext();
        entry.Version = Convert.ToUInt16(nodes.Current.Value);
        
        string pathToRead = Path.Join(path, file);
        entry.Data = File.ReadAllBytes(pathToRead);
        entry.SlotRamRequired = (uint)(entry.Data.Length);

        //finish
        sourceDataDescriptionNode.InnerText = "not available";
        return entry;
    }
}