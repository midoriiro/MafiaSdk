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

using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Xml;

public class Xml
{
    public bool Unk1 { get; set; }
    public bool HasFailedToDecompile { get; set; }
    public string Content { get; set; } = null!;
    
    private Xml()
    {
    }

    public static Xml Read(Stream stream, Endian endian)
    {
        bool unk1 = stream.ReadByte() > 0;
        
        string content = null!;
        var hasFailedToDecompile = false;
        
        // Super hacky solution to unpack XMLs with xml:xsi etc.
        if (unk1 == false)
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

        return new Xml()
        {
            Unk1 = unk1,
            HasFailedToDecompile = hasFailedToDecompile,
            Content = content
        };
    }
    
    public void Write(Stream stream, Endian endian)
    {
        // TODO refactor these two (de)serializers
        if (Unk1 == false)
        {
            long currentPosition = stream.Position;
            
            if (!HasFailedToDecompile)
            {
                XmlResource0.Serialize(stream, Content, endian);
            }
            else
            {
                // TODO ?
                stream.Position = currentPosition;
                byte[] data = File.ReadAllBytes(Content);
                stream.WriteBytes(data);
            }
        }
        else
        {
            XmlResource1.Serialize(stream, Content, endian);
        }
    }
}