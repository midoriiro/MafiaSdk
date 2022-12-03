namespace Core.IO.Streams;

public static class Assert
{
    private static InvalidOperationException CreateException<T>(T value, T[] expected)
    {
        return new InvalidOperationException(
            $"Value '{value}' is not equal to one of these values '{string.Join(",", expected)}'"
        );
    }
    
    public static T AssertValue<T>(this T value, params T[] expected)
    {
        if (!expected.Contains(value))
        {
            throw CreateException(value, expected);
        }

        return value;
    }
    
    public static ushort AssertValue(this ushort value, params ushort[] expected)
    {
        if (!expected.Contains(value))
        {
            throw CreateException(value, expected);
        }

        return value;
    }
    
    public static byte AssertValue(this byte value, params byte[] expected)
    {
        if (!expected.Contains(value))
        {
            throw CreateException(value, expected);
        }

        return value;
    }
    
    public static string AssertValue(this string value, uint length)
    {
        if (value.Length != length)
        {
            throw new InvalidOperationException(
                $"String value '{value}' is not equal to length of '{length}', acu"
            );
        }

        return value;
    }
}