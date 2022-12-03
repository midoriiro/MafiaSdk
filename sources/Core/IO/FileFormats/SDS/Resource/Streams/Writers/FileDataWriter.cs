using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Writers;

public class FileDataWriter : IDataWriter
{
    public void Write(DataDescriptor dataDescriptor)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dataDescriptor.Path)!);
        using var stream = new FileStream(dataDescriptor.Path, new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Options = FileOptions.Asynchronous
        });
        stream.Write(dataDescriptor.Data);
    }
}