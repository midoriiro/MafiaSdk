using System.Xml;
using System.Xml.XPath;

namespace Core.IO.FileFormats.SDS.Resource.Types.Xml0;

internal class NodeEntry
{
    public DataValue Name { get; set; } = null!;
    public DataValue? Value { get; set; }
    public uint Id { get; set; }
    public readonly List<uint> Children = new();
    public readonly List<AttributeEntry> Attributes = new();

    internal static void ReadXmlNode(List<NodeEntry> nodes, NodeEntry parent, XPathNavigator? nav)
    {
        if (nav is null)
        {
            return;
        }
        
        var node = new NodeEntry()
        {
            Name = new DataValue(DataType.String, nav.Name),
            Value = null!,
            Id = (uint)nodes.Count,
        };

        parent.Children.Add(node.Id);
        nodes.Add(node);

        var type = DataType.String;

        if (nav.MoveToFirstAttribute())
        {
            do
            {
                if (nav.Name == "__type")
                {
                    type = DataTypeFromString(nav.Value);
                    continue;
                }

                node.Attributes.Add(new AttributeEntry()
                {
                    Name = new DataValue(DataType.String, nav.Name),
                    Value = new DataValue(DataType.String, nav.Value),
                });
            } while (nav.MoveToNextAttribute());

            nav.MoveToParent();
        }

        XPathNodeIterator children = nav.SelectChildren(XPathNodeType.Element);

        if (children.Count > 0)
        {
            while (children.MoveNext())
            {
                ReadXmlNode(nodes, node, children.Current);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(nav.Value) == false)
            {
                node.Value = new DataValue(type, nav.Value);
            }
        }
    }

    internal static void WriteXmlNode(XmlWriter writer, List<NodeEntry> nodes, NodeEntry node)
    {
        writer.WriteStartElement(node.Name.ToString());

        foreach (AttributeEntry attribute in node.Attributes)
        {
            writer.WriteStartAttribute(attribute.Name.ToString());
            writer.WriteValue(attribute.Value is null ? "" : attribute.Value.ToString());
            writer.WriteEndAttribute();
        }

        foreach (uint childId in node.Children)
        {
            NodeEntry? child = nodes.SingleOrDefault(n => n.Id == childId);

            if (child is null)
            {
                throw new KeyNotFoundException();
            }

            WriteXmlNode(writer, nodes, child);
        }

        if (node.Value is not null)
        {
            if (node.Value.Type != DataType.String)
            {
                writer.WriteAttributeString("__type", DataTypeToString(node.Value.Type));
            }

            writer.WriteValue(node.Value.ToString());
        }

        writer.WriteEndElement();
    }

    private static string DataTypeToString(DataType type)
    {
        switch (type)
        {
            case DataType.Special:
            {
                return "x";
            }

            case DataType.Boolean:
            {
                return "b";
            }

            case DataType.Float:
            {
                return "f";
            }

            case DataType.String:
            {
                return "s";
            }

            case DataType.Integer:
            {
                return "i";
            }
            case DataType.Unknown:
            {
                return "u";
            }
        }

        throw new NotSupportedException();
    }

    private static DataType DataTypeFromString(string type)
    {
        switch (type)
        {
            case "x":
            {
                return DataType.Special;
            }

            case "b":
            {
                return DataType.Boolean;
            }

            case "f":
            {
                return DataType.Float;
            }

            case "s":
            {
                return DataType.String;
            }

            case "i":
            {
                return DataType.Integer;
            }

            case "u":
            {
                return DataType.Unknown;
            }
        }

        throw new NotSupportedException();
    }
}