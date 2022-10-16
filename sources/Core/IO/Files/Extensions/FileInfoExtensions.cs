namespace Core.IO.Files.Extensions;

public static class FileInfoExtensions
{
    public static string ConvertToMemorySize(this long value)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = value;
        var order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
        // show a single decimal place, and no space.
        return $"{len:0.##} {sizes[order]}";
    }

    public static string CalculateFileSize(this FileInfo file)
    {
        return file.Length.ConvertToMemorySize();
    }
}