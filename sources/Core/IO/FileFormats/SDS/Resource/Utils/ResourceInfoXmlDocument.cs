using System.Text;
using System.Xml;

namespace Core.IO.FileFormats.SDS.Resource.Utils;

public sealed class ResourceInfoXmlDocument : IDisposable
{
    private readonly StringBuilder _stringBuilder = new();
    private readonly StringWriter _stringWriter;
    private readonly XmlTextWriter _xmlWriter;

    public ResourceInfoXmlDocument()
    {
        _stringWriter = new StringWriter(_stringBuilder);
        _xmlWriter = new XmlTextWriter(_stringWriter)
        {
            Formatting = Formatting.Indented,
            IndentChar = '\t',
            Indentation = 1,
            QuoteChar = '\''
        };
    }

    public string WriteDocument(XmlDocument document)
    {
        document.Save(_xmlWriter);
        return _stringBuilder.ToString();
    }

    public void Dispose()
    {
        _stringWriter.Dispose();
        _xmlWriter.Dispose();
    }
}