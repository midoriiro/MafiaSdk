namespace Core.IO.FileFormats.SDS.Resource.Entries.Extensions;

public static class ListExtensions
{
    public static int[] FindIndexes<T>(this List<T> list, List<T> items)
    {
        var indexes = new int[items.Count];
        var lastIndex = 0;
        
        for (var index = 0; index < items.Count; index++)
        {
            T item = items[index];
            int itemIndex = list.IndexOf(item, lastIndex);

            if (itemIndex == -1)
            {
                throw new InvalidOperationException("Index not found");
            }

            lastIndex = itemIndex + 1;
            indexes[index] = itemIndex;
        }

        return indexes;
    }
}