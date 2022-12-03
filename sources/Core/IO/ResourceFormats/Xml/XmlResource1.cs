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

using System.Text;
using System.Xml;
using Core.IO.ResourceFormats.Xml.Xml1;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Xml
{
    public class XmlResource1
    {
        public static void Serialize(Stream stream, string content, Endian endian)
        {
            // Serialize header. It is BX with a 1 after.
            stream.WriteValueU16(0x5842, endian);
            stream.WriteValueU16(1, endian);

            // Read the XMLDocument.
            var document = new XmlDocument();
            document.LoadXml(File.ReadAllText(content));
            XmlNodeList childNodes = document.ChildNodes;
            NodeEntry.Serialize(stream, childNodes, endian);
        }

        public static string Deserialize(Stream stream, Endian endian)
        {
            if (stream.ReadValueU16(endian) != 0x5842) // 'BX'
            {
                throw new FormatException();
            }

            if (stream.ReadValueU16(endian) > 1)
            {
                throw new FormatException();
            }

            var root = (NodeEntry)NodeEntry.Deserialize(stream, endian);

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, settings);

            writer.WriteStartDocument();
            NodeEntry.WriteXmlNode(writer, root);
            writer.WriteEndDocument();

            writer.Flush();
            return output.ToString();
        }
    }
}
