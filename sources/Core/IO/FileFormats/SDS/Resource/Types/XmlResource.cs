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

public class XmlResource : IResourceType<XmlResource>
{
    public string Tag { get; set; } = null!;
    public byte? Unk0 { get; set; }
    public string Name { get; set; } = null!;
    public byte? Unk1 { get; set; }
    [IgnoreFieldDescriptor]
    public byte[] Data { get; set; } = null!;

    private XmlResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteStringU32(Tag, endian);

        if (version >= 3)
        {
            stream.WriteValueU8(Unk0!.Value);
        }

        stream.WriteStringU32(Name, endian);

        if (version >= 2)
        {
            stream.WriteValueU8(Unk1!.Value);
        }
        
        using var dataStream = new MemoryStream(Data);
        dataStream.ReadByte();
        byte[] data = dataStream.ReadBytes((int)(dataStream.Length - dataStream.Position));

        stream.WriteBytes(data);
    }

    public static XmlResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        string tag = stream.ReadStringU32(endian);
        byte? unk0 = version >= 3 ? stream.ReadValueU8() : null;
        string name = stream.ReadStringU32(endian);
        byte? unk1 = version >= 2 ? stream.ReadValueU8() : null;
        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));
        
        using var dataStream = new MemoryStream();
        dataStream.WriteByte((byte)(unk1.HasValue ? unk1.Value != 0 ? 1 : 0 : 0));
        dataStream.WriteBytes(data);

        return new XmlResource
        {
            Tag = tag,
            Unk0 = unk0,
            Name = name,
            Unk1 = unk1,
            Data = dataStream.ToArray()
        };
    }
}