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
using System.Xml.XPath;
using Core.IO.FileFormats.SDS.Resource.Types.Xml0;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types
{
    public class XmlResource0
    {
        public static void Serialize(Stream output, string content, Endian endian)
        {
            var nodes = new List<NodeEntry>();
            var xml = new XPathDocument(content);
            XPathNavigator nav = xml.CreateNavigator();

            nav.MoveToRoot();

            XPathNodeIterator children = nav.SelectChildren(XPathNodeType.Element);
            
            if (children.Count != 1 || children.MoveNext() == false)
            {
                throw new InvalidOperationException();
            }

            var root = new NodeEntry()
            {
                Name = null!,
                Value = null,
                Id = 0,
            };
            
            nodes.Add(root);

            NodeEntry.ReadXmlNode(nodes, root, children.Current);

            var valueData = new MemoryStream();
            valueData.WriteValueU32(4, endian);
            valueData.WriteValueU32(0, endian);
            valueData.WriteValueU8(0);

            var nodeData = new MemoryStream();

            foreach (NodeEntry node in nodes)
            {
                nodeData.WriteValueU32(DataValue.Serialize(valueData, node.Name, endian), endian);
                nodeData.WriteValueU32(DataValue.Serialize(valueData, node.Value, endian), endian);
                nodeData.WriteValueU32(node.Id, endian);

                nodeData.WriteValueS32(node.Children.Count, endian);
                
                foreach (uint child in node.Children)
                {
                    nodeData.WriteValueU32(child, endian);
                }

                nodeData.WriteValueS32(node.Attributes.Count, endian);
                
                foreach (AttributeEntry attribute in node.Attributes)
                {
                    nodeData.WriteValueU32(DataValue.Serialize(valueData, attribute.Name, endian), endian);
                    nodeData.WriteValueU32(DataValue.Serialize(valueData, attribute.Value, endian), endian);
                }
            }

            valueData.Position = 0;
            nodeData.Position = 0;

            output.WriteValueU8(4);
            output.WriteValueS32(nodes.Count, endian);
            output.WriteValueU32((uint)valueData.Length, endian);
            output.WriteFromStream(valueData, (uint)valueData.Length);
            output.WriteFromStream(nodeData, nodeData.Length);
        }

        public static string Deserialize(Stream input, Endian endian)
        {
            if (input.ReadValueU8() != 4)
            {
                throw new FormatException();
            }

            uint count = input.ReadValueU32(endian);
            
            if (count > 0x3FFFFFFF)
            {
                throw new FormatException();
            }

            uint dataSize = input.ReadValueU32(endian);
            
            if (dataSize > 0x500000)
            {
                throw new FormatException();
            }
            
            MemoryStream data = input.ReadToMemoryStream((int)dataSize);

            var nodes = new List<NodeEntry>();
            
            for (uint i = 0; i < count; i++)
            {
                var node = new NodeEntry()
                {
                    Name = DataValue.Deserialize(data, input.ReadValueU32(endian), endian)!,
                    Value = DataValue.Deserialize(data, input.ReadValueU32(endian), endian),
                    Id = input.ReadValueU32(endian)
                };

                uint childCount = input.ReadValueU32(endian);
                node.Children.Clear();
                
                for (uint j = 0; j < childCount; j++)
                {
                    node.Children.Add(input.ReadValueU32(endian));
                }

                uint attributeCount = input.ReadValueU32(endian);
                node.Attributes.Clear();
                
                for (uint j = 0; j < attributeCount; j++)
                {
                    var attribute = new AttributeEntry()
                    {
                        Name = DataValue.Deserialize(data, input.ReadValueU32(endian), endian)!,
                        Value = DataValue.Deserialize(data, input.ReadValueU32(endian), endian),
                    };
                    
                    if (attribute.Name.Value.ToString() == "__type")
                    {
                        throw new FormatException("someone used __type?");
                    }

                    node.Attributes.Add(attribute);
                }

                nodes.Add(node);
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, settings);
            writer.WriteStartDocument();

            if (nodes.Count > 0)
            {
                NodeEntry? root = nodes.SingleOrDefault(n => n.Id == 0);

                if (root == null)
                {
                    throw new InvalidOperationException();
                }

                if (root.Children.Count != 1 || root.Attributes.Count > 0 || root.Value != null)
                {
                    throw new FormatException();
                }

                foreach (uint childId in root.Children)
                {
                    NodeEntry? child = nodes.SingleOrDefault(n => n.Id == childId);
                    if (child == null)
                    {
                        throw new KeyNotFoundException();
                    }

                    NodeEntry.WriteXmlNode(writer, nodes, child);
                }
            }
            
            writer.WriteEndDocument();
            writer.Flush();
            
            return output.ToString();
        }
    }
}
