using Core.IO.FileFormats.SDS.Archive;

namespace Core.IO.FileFormats.SDS.Resource.Caches;

public class ResourceNameCache
{
    private readonly Dictionary<ulong, string> _cache;

    public string this[ulong key]
    {
        get => _cache[key];
    }

    private ResourceNameCache(Dictionary<ulong, string> cache)
    {
        _cache = cache;
    }

    public static ResourceNameCache FromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException($"Path '{path}' do not exists", nameof(path));
        }

        string[] lines = File.ReadAllLines(path);
        
        Dictionary<ulong, string> cache = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] splitLine = line.Split(' ');
            ulong nameHash = ulong.Parse(splitLine.First());
            string name = splitLine.Last();

            if (!cache.ContainsKey(nameHash))
            {
                cache.Add(nameHash, name);
            }
        }

        return new ResourceNameCache(cache);
    }

    public void ToFile(string path)
    {
        string[] lines = _cache
            .Select(entry => $"{entry.Key} {entry.Value}")
            .ToArray();

        File.WriteAllLines(path, lines);
    }

    public string? HasFilename(ResourceEntry entry)
    {
        bool hasFilename = _cache.TryGetValue(entry.FileHash, out string? value);
        return hasFilename ? value : null;
    }

    public bool ContainsKey(ulong key)
    {
        return _cache.ContainsKey(key);
    }
}