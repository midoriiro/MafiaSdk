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

using Core.Games;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Entries.Extensions;
using Core.IO.FileFormats.SDS.Resource.Manifest;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Entries;

public class TextureEntry : IResourceEntry
{
    public static EntryDeserializeResult Deserialize(
        ResourceEntry resourceEntry,
        string name,
        Endian endian
    )
    {
        TextureResource resource;

        using (var stream = new MemoryStream(resourceEntry.Data!))
        {
            resource = TextureResource.Deserialize(resourceEntry.Version, stream, endian);
        }

        ManifestEntryDescriptors resourceDescriptors = ManifestEntryDescriptors.FromResource(resource);
        resourceDescriptors.AddFileName(name);

        return new EntryDeserializeResult
        {
            ManifestEntryDescriptors = resourceDescriptors,
            DataDescriptors = new[] { new DataDescriptor(name, resource.Data) }
        };
    }

    public static EntrySerializeResult Serialize(
        ManifestEntry manifestEntry,
        string path,
        Endian endian
    )
    {
        var resource = manifestEntry.Descriptors.ToResource<TextureResource>();
        string filename = manifestEntry.Descriptors.GetFilename()!;
        ushort version = manifestEntry.MetaData.Version;
        
        string pathToRead = Path.Join(path, filename);
        resource.Data = File.ReadAllBytes(pathToRead);

        var resourceEntry = new ResourceEntry
        {
            Version = manifestEntry.MetaData.Version,
            TypeId = manifestEntry.MetaData.Type.Id,
            FileHash = manifestEntry.MetaData.FileHash // TODO compute that
        };

        using (var stream = new MemoryStream())
        {
            resource.Serialize(version, stream, endian);
            resourceEntry.Data = stream.ToArray();
        }
        
        if (GameWorkSpace.Instance().SelectedGame.Type is GamesEnumerator.Mafia2 or GamesEnumerator.Mafia2DefinitiveEdition)
        {
            resourceEntry.SlotVramRequired = manifestEntry.MetaData.SlotVramRequired;
            // TODO
            // resourceEntry.SlotVramRequired = (uint)(resource.Data.Length - 128);
            //
            // if (resource.HasMipMap)
            // {
            //     var fileInfo = new FileInfo(Path.Join(path, $"MIP_{filename}"));
            //     resourceEntry.SlotVramRequired += (uint)fileInfo.Length - 128;
            // }
        }
        else
        {
            int size = resource.IsDirectX10 ? 157 : 137;
            resourceEntry.SlotVramRequired = manifestEntry.MetaData.SlotVramRequired; // TODO find correct value
        }

        return new EntrySerializeResult
        {
            DataDescriptor = filename.RemoveDeduplicationMark(),
            ResourceEntry = resourceEntry
        };
    }
}