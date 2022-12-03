using System.Xml;
using System.Xml.Linq;
using Core.IO.FileFormats.SDS;
using Core.IO.FileFormats.SDS.Archive;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;

namespace Core.Tests.IO.FileFormats.SDS;

public sealed class ArchiveFileEqualityComparer : IEqualityComparer<ArchiveFile>
{
    private static readonly ByteArrayComparer ByteArrayComparer = new();
    private static readonly ResourceTypeEqualityComparer ResourceTypeEqualityComparer = new();
    private static readonly ResourceEntryEqualityComparer ResourceEntryEqualityComparer = new();
    
    public bool Equals(ArchiveFile? x, ArchiveFile? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || x.GetType() != y.GetType())
        {
            return false;
        }

        if (x.ResourceInfoXml is null && y.ResourceInfoXml is not null)
        {
            return false;
        }

        if (x.ResourceInfoXml is not null && y.ResourceInfoXml is not null)
        {
            XDocument xDocument = XDocument.Parse(x.ResourceInfoXml!);
            XDocument yDocument = XDocument.Parse(y.ResourceInfoXml!);

            if (!XNode.DeepEquals(xDocument, yDocument))
            {
                DiffPaneModel? differ = InlineDiffBuilder.Diff(x.ResourceInfoXml, y.ResourceInfoXml);

                int linesCount = differ.Lines.Count;
                int lineWidth = linesCount.ToString().Length;

                for (var index = 0; index < linesCount; index++)
                {
                    DiffPiece? line = differ.Lines[index];

                    if (line.Text.Contains("CustomDebugInfo") || line.Type != ChangeType.Inserted)
                    {
                        continue;
                    }

                    int startIndex = index - 5 < 0 ? 0 : index - 5;
                    int endIndex = index + 5;
                    
                    Console.Out.WriteLine("########################################");

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        DiffPiece? lineToPrint = differ.Lines[i];
                        ConsoleColor color = lineToPrint.Type switch
                        {
                            ChangeType.Unchanged => ConsoleColor.DarkGray,
                            ChangeType.Deleted => ConsoleColor.DarkRed,
                            ChangeType.Inserted => ConsoleColor.DarkGreen,
                            ChangeType.Imaginary => ConsoleColor.DarkMagenta,
                            ChangeType.Modified => ConsoleColor.DarkCyan,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        
                        string lineNumber = index.ToString().PadLeft(lineWidth);
                        Console.ForegroundColor = color;
                        Console.Out.Write($"{lineNumber} |");
                        Console.Out.WriteLine($"{lineToPrint.Text}");
                    }
                    
                    Console.Out.WriteLine("########################################");
                }
                
                return false;
            }
        }

        if (x.ResourceEntries.Count != y.ResourceEntries.Count)
        {
            return false;
        }

        for (var index = 0; index < x.ResourceEntries.Count; index++)
        {
            ResourceEntry xEntry = x.ResourceEntries[index];
            ResourceEntry yEntry = y.ResourceEntries[index];

            if (!ResourceEntryEqualityComparer.Equals(xEntry, yEntry))
            {
                return false;
            }
        }

        bool result =  x.Endian == y.Endian &&
                       x.Platform == y.Platform &&
                       x.Version == y.Version &&
                       x.SlotRamRequired == y.SlotRamRequired &&
                       x.SlotVramRequired == y.SlotVramRequired &&
                       // x.OtherRamRequired == y.OtherRamRequired && // TODO fix this
                       // x.OtherVramRequired == y.OtherVramRequired &&
                       ByteArrayComparer.Equals(x.Unknown20, y.Unknown20) &&
                       x.ResourceTypes.SequenceEqual(y.ResourceTypes, ResourceTypeEqualityComparer);

        return result;
    }

    public int GetHashCode(ArchiveFile obj)
    {
        var hashCode = new HashCode();
        hashCode.Add((int)obj.Endian);
        hashCode.Add(obj.Version);
        hashCode.Add((int)obj.Platform);
        hashCode.Add(obj.SlotRamRequired);
        hashCode.Add(obj.SlotVramRequired);
        hashCode.Add(obj.OtherRamRequired);
        hashCode.Add(obj.OtherVramRequired);
        hashCode.Add(obj.Unknown20);
        hashCode.Add(obj.ResourceInfoXml);
        hashCode.Add(obj.ResourceTypes);
        hashCode.Add(obj.ResourceEntries);
        hashCode.Add(obj.ResourceNames);
        return hashCode.ToHashCode();
    }
}

public sealed class ResourceTypeEqualityComparer : IEqualityComparer<ResourceType>
{
    public bool Equals(ResourceType x, ResourceType y)
    {
        bool result = x.Id == y.Id && x.Name == y.Name && x.Parent == y.Parent;
        return result;
    }

    public int GetHashCode(ResourceType obj)
    {
        return HashCode.Combine(obj.Id, obj.Name, obj.Parent);
    }
}

public sealed class ResourceEntryEqualityComparer : IEqualityComparer<ResourceEntry>
{
    private static readonly ByteArrayComparer ByteArrayComparer = new();
    
    public bool Equals(ResourceEntry? x, ResourceEntry? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || x.GetType() != y.GetType())
        {
            return false;
        }

        bool result = x.TypeId == y.TypeId &&
                      x.Version == y.Version &&
                      ByteArrayComparer.Equals(x.Data, y.Data) &&
                      x.FileHash == y.FileHash &&
                      x.SlotRamRequired == y.SlotRamRequired &&
                      x.SlotVramRequired == y.SlotVramRequired;
                      // x.OtherRamRequired == y.OtherRamRequired && // TODO fix this
                      // x.OtherVramRequired == y.OtherVramRequired
                      ;
        return result;
    }

    public int GetHashCode(ResourceEntry obj)
    {
        return HashCode.Combine(
            obj.TypeId,
            obj.Version,
            obj.Data,
            obj.FileHash,
            obj.SlotRamRequired,
            obj.SlotVramRequired,
            obj.OtherRamRequired,
            obj.OtherVramRequired
        );
    }
}

public sealed class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || x.GetType() != y.GetType())
        {
            return false;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (var index = 0; index < x.Length; index++)
        {
            byte xByte = x[index];
            byte yByte = y[index];

            if (xByte != yByte)
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        return obj.GetHashCode();
    }
}