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

using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class ScriptEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        ScriptResource resource;

        using (var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = ScriptResource.Deserialize(resourceEntry.Version, stream, endian);
        }
        
        ManifestEntryDescriptors resourceDescriptors = ManifestEntryDescriptors.FromResource(resource);
        resourceDescriptors.AddFileName(name);

        var data = new DataDescriptor[resource.ScriptsCount];

        for (var index = 0; index != resource.ScriptsCount; index++)
        {
            ScriptData script = resource.Scripts[index];
            data[index] = new DataDescriptor(script.Name, script.Data); // TODO Check if extension or path is valid in name
        }

        return new EntryDeserializeResult
        {
            ManifestEntryDescriptors = resourceDescriptors,
            DataDescriptors = data
        };
    }

    public static EntrySerializeResult Serialize(
        ManifestEntry manifestEntry,
        string path,
        Endian endian
    )
    {
        var resource = manifestEntry.Descriptors.ToResource<ScriptResource>();
        ushort version = manifestEntry.MetaData.Version;

        for (var index = 0; index < resource.ScriptsCount; index++)
        {
            ScriptData script = resource.Scripts[index];
            string pathToRead = Path.Join(path, script.Name);
            script.Data = File.ReadAllBytes(pathToRead);
        }

        var resourceEntry = new ResourceEntry
        {
            Version = manifestEntry.MetaData.Version,
            TypeId = manifestEntry.MetaData.Type.Id,
            FileHash = manifestEntry.MetaData.FileHash, // TODO compute that
            SlotRamRequired = manifestEntry.MetaData.SlotRamRequired // TODO find correct value
        };

        using (var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            resourceEntry.Data = stream.ToArray();
        }

        return new EntrySerializeResult
        {
            DataDescriptor = "not available",
            ResourceEntry = resourceEntry
        };
    }
}