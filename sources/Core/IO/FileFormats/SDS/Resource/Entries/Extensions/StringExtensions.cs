namespace Core.IO.FileFormats.SDS.Resource.Entries.Extensions;

public static class StringExtensions
{
    // TODO use read only span
    public static string RemoveExtension(this string path)
    {
        int lastPeriod = path.LastIndexOf('.');
        return lastPeriod < 0 ?
            path : // No extension was found
            path[..lastPeriod];
    }
    
    // TODO use read only span
    public static string RemoveDeduplicationMark(this string filename)
    {
        string extension = Path.GetExtension(filename);
        string name = Path.GetFileNameWithoutExtension(filename);
        
        int markIndex = name.IndexOf("#", StringComparison.Ordinal);

        if (markIndex == -1)
        {
            return filename;
        }

        name = name.Remove(markIndex, name.Length - markIndex);
        return $"{name}{extension}";
    }
}