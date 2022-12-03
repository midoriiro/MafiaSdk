using System.IO.MemoryMappedFiles;
using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Streams.Writers;

namespace Core.Tests.Utils;

public class NullDataWriter : IDataWriter
{
    public void Write(DataDescriptor dataDescriptor)
    {
    }
}