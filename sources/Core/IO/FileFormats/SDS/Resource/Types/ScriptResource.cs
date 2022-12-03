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

using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class ScriptResource : IResourceType<ScriptResource>
{
    public string Path { get; set; } = null!;
    [IgnoreFieldDescriptor]
    public int ScriptsCount => Scripts.Length;

    public ScriptData[] Scripts { get; set; } = null!;

    private ScriptResource()
    {
    }

    // TODO move this to entry class ?
    public uint GetDataLength()
    {
        return (uint)Scripts.Sum(script => script.Data.Length);
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteStringU16(Path, endian);
        stream.WriteValueS32(Scripts.Length, endian);
        
        foreach (ScriptData script in Scripts)
        {
            script.Serialize(version, stream, endian);
        }
    }

    public static ScriptResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        string path = stream.ReadStringU16(endian);
        uint count = stream.ReadValueU32(endian);

        var scripts = new ScriptData[count];

        for (uint index = 0; index < count; index++)
        {
            scripts[index] = ScriptData.Deserialize(version, stream, endian);
        }

        return new ScriptResource
        {
            Path = path,
            Scripts = scripts
        };
    }
}