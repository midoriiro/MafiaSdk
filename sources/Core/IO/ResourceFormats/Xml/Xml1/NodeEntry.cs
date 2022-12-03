using System.Text;
using System.Xml;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Xml.Xml1;

internal class NodeEntry
{
    public string Name { get; set; } = null!;
    public DataValue? Value { get; set; }
    public List<NodeEntry> Children { get; } = new();
    public List<AttributeEntry> Attributes { get; } = new();
    
    internal static void WriteXmlNode(XmlWriter writer, NodeEntry node)
    {
        writer.WriteStartElement(node.Name);

        foreach (AttributeEntry attribute in node.Attributes)
        {
            writer.WriteStartAttribute(attribute.Name);
            writer.WriteValue(attribute.Value is null
                ? ""
                : attribute.Value.ToString());
            writer.WriteEndAttribute();
        }

        foreach (NodeEntry child in node.Children)
        {
            WriteXmlNode(writer, child);
        }

        if (node.Value is not null)
        {
            writer.WriteValue(node.Value.ToString());
        }

        writer.WriteEndElement();
    }

    internal static void Serialize(Stream stream, XmlNodeList? childNodes, Endian endian)
    {
        if (childNodes is null)
        {
            return;
        }

        foreach (XmlNode childNode in childNodes)
        {
            switch (childNode.NodeType)
            {
                case XmlNodeType.Text when childNode.Value is not null:
                    stream.WriteValueU16((ushort)(childNode.Value.Length + 1), endian);
                    stream.WriteValueU8(4);
                    stream.WriteValueU8(0);
                    stream.WriteValueU16((ushort)childNode.Value.Length, endian);
                    stream.WriteStringZ(childNode.Value);
                    break;
                case XmlNodeType.Element:
                {
                    string name = childNode.Name;
                    int numChildren = childNode.ChildNodes.Count;
                    int numAttributes = childNode.Attributes?.Count ?? 0;
                    int numBytes = name.Length + 1 + 16 * numChildren + 16 * numAttributes;
                    stream.WriteValueU16((ushort)numBytes, endian);
                    stream.WriteValueU8(1);
                    stream.WriteValueU8((byte)name.Length);
                    stream.WriteValueU16((ushort)numChildren, endian);
                    stream.WriteValueU8((byte)numAttributes);
                    stream.WriteStringZ(name);

                    if (childNode.HasChildNodes)
                    {
                        Serialize(stream, childNode.ChildNodes, endian);
                    }

                    if (childNode.Attributes is null)
                    {
                        continue;
                    }

                    foreach (XmlAttribute attributeNode in childNode.Attributes)
                    {
                        stream.WriteValueU16((ushort)(attributeNode.Name.Length + 1), endian);
                        stream.WriteValueU8(5);
                        stream.WriteValueU8((byte)attributeNode.Name.Length);
                        stream.WriteStringZ(attributeNode.Name);

                        stream.WriteValueU16((ushort)(attributeNode.Value.Length + 1), endian);
                        stream.WriteValueU8(4);
                        stream.WriteValueU8(0);
                        stream.WriteValueU16((ushort)attributeNode.Value.Length);
                        stream.WriteStringZ(attributeNode.Value);
                    }

                    break;
                }
            }
        }
    }

    internal static object Deserialize(Stream stream, Endian endian)
    {
        stream.ReadValueU16(endian); // unk1
        byte nodeType = stream.ReadValueU8();

        switch (nodeType)
        {
            case 1:
            {
                byte nameLength = stream.ReadValueU8();
                ushort childCount = stream.ReadValueU16(endian);
                byte attributeCount = stream.ReadValueU8();

                string name = stream.ReadString(nameLength + 1, true, Encoding.UTF8);

                var node = new NodeEntry()
                {
                    Name = name,
                };

                var children = new List<object>();
                
                for (ushort i = 0; i < childCount; i++)
                {
                    children.Add(Deserialize(stream, endian));
                }

                if (children.Count == 1 && children[0] is DataValue)
                {
                    node.Value = (DataValue)children[0];
                }
                else
                {
                    foreach (object child in children)
                    {
                        node.Children.Add((NodeEntry)child);
                    }
                }

                for (byte i = 0; i < attributeCount; i++)
                {
                    object child = Deserialize(stream, endian);

                    if (child is NodeEntry data)
                    {
                        if (data.Children.Count != 0 || data.Attributes.Count != 0)
                        {
                            throw new FormatException();
                        }

                        var attribute = new AttributeEntry()
                        {
                            Name = data.Name,
                            Value = data.Value,
                        };

                        node.Attributes.Add(attribute);
                    }
                    else
                    {
                        node.Attributes.Add((AttributeEntry)child);
                    }
                }

                return node;
            }

            case 4:
            {
                byte valueType = stream.ReadValueU8();

                if (valueType != 0)
                {
                    throw new NotImplementedException();
                }

                ushort valueLength = stream.ReadValueU16(endian);
                string value = stream.ReadString(valueLength + 1, true, Encoding.UTF8);
                return new DataValue(DataType.String, value);
            }

            case 5:
            {
                byte nameLength = stream.ReadValueU8();
                string name = stream.ReadString(nameLength + 1, true, Encoding.UTF8);

                var attribute = new NodeEntry
                {
                    Name = name,
                    Value = (DataValue)Deserialize(stream, endian)
                };

                return attribute;
            }

            default:
            {
                throw new NotImplementedException();
            }
        }
    }
}