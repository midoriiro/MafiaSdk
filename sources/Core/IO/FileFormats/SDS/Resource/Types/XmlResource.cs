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

using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class XmlResource : IResourceType<XmlResource>
{
    public string Tag { get; set; } = null!;
    public bool Unk1 { get; set; }
    public string Name { get; set; } = null!;
    public bool Unk3 { get; set; }
    public string Content { get; set; } = null!;
    public bool HasFailedToDecompile { get; set; }

    internal XmlResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteStringU32(Tag, endian);

        if (version >= 3)
        {
            stream.WriteValueU8((byte)(Unk1 ? 1 : 0));
        }

        stream.WriteStringU32(Name, endian);

        if (version >= 2)
        {
            stream.WriteValueU8((byte)(Unk3 ? 1 : 0));
        }

        if (Unk3 == false)
        {
            if (!HasFailedToDecompile)
            {
                XmlResource0.Serialize(stream, Content, endian);
            }
            else
            {
                byte[] data = File.ReadAllBytes(Content);
                stream.WriteBytes(data);
            }
        }
        else
        {
            XmlResource1.Serialize(stream, Content, endian);
        }
    }

    public static XmlResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        string tag = stream.ReadStringU32(endian);
        bool unk1 = version >= 3 ? stream.ReadValueU8() != 0 : true;
        string name = stream.ReadStringU32(endian);
        bool unk3 = version >= 2 ? stream.ReadValueU8() != 0 : false;

        string content = null!;
        var hasFailedToDecompile = false;

        // Super hacky solution to unpack XMLs with xml:xsi etc.
        if (unk3 == false)
        {
            long currentPosition = stream.Position;

            try
            {
                content = XmlResource0.Deserialize(stream, endian);
            }
            catch (Exception exception)
            {
                stream.Position = currentPosition;
                // Console.WriteLine(ex.Message); // TODO throw exception
                hasFailedToDecompile = true;
            }
        }
        else
        {
            content = XmlResource1.Deserialize(stream, endian);
        }

        return new XmlResource()
        {
            Tag = tag,
            Unk1 = unk1,
            Name = name,
            Unk3 = unk3,
            Content = content,
            HasFailedToDecompile = hasFailedToDecompile
        };
    }
}