using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.FileFormats.SDS.Resource.Types;

namespace Core.IO.FileFormats.SDS.Resource.Manifest;

public class ManifestEntryDescriptors
{
    private static readonly Type IgnoreFieldDescriptorType = typeof(IgnoreFieldDescriptorAttribute);
    private static readonly Type ResourceInterfaceType = typeof(IResourceType<>);
    private static readonly Type StringType = typeof(string);
    private static readonly Type Int16Type = typeof(short);
    private static readonly Type Int32Type = typeof(int);
    private static readonly Type Int64Type = typeof(long);
    private static readonly Type UInt16Type = typeof(ushort);
    private static readonly Type UInt32Type = typeof(uint);
    private static readonly Type UInt64Type = typeof(ulong);
    private static readonly Type NullableUInt64Type = typeof(ulong?);
    private static readonly Type ByteType = typeof(byte);
    private static readonly Type ByteArrayType = typeof(byte[]);
    private static readonly Type NullableByteType = typeof(byte?);
    private static readonly Type BooleanType = typeof(bool);

    private static readonly Type[] NumberTypes =
    {
        Int16Type,
        Int32Type,
        Int64Type,
        UInt16Type,
        UInt32Type,
        UInt64Type,
        NullableUInt64Type
    };

    public Dictionary<string, object> Descriptors { get; init; } = new();

    internal ManifestEntryDescriptors()
    {
    }
    
    private static Dictionary<string, object> GetDescriptors(object resource)
    {
        return resource
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(propertyInfo => propertyInfo.CustomAttributes.All(attribute => attribute.AttributeType != IgnoreFieldDescriptorType))
            .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => GetValue(propertyInfo, resource));
    }
    
    private void SetDescriptors(object @object)
    {
        var properties = @object
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(propertyInfo => propertyInfo.CustomAttributes.All(attribute => attribute.AttributeType != IgnoreFieldDescriptorType))
            .ToArray();

        foreach (PropertyInfo propertyInfo in properties)
        {
            object value = Descriptors[propertyInfo.Name];
            SetValue(propertyInfo, @object, value);   
        }
    }

    private void SetDescriptors(object @object, IReadOnlyDictionary<string, object> values)
    {
        var properties = @object
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(propertyInfo => propertyInfo.CustomAttributes.All(attribute => attribute.AttributeType != IgnoreFieldDescriptorType))
            .ToArray();

        foreach (PropertyInfo propertyInfo in properties)
        {
            object value = values[propertyInfo.Name];
            SetValue(propertyInfo, @object, value);   
        }
    }

    private static object GetValue(PropertyInfo propertyInfo, object @object)
    {
        Type? declaringType = propertyInfo.DeclaringType;
        Type propertyType = propertyInfo.PropertyType;

        if (propertyType.IsValueType || propertyType == StringType || propertyType == ByteArrayType)
        {
            return propertyInfo.GetValue(@object)!;
        }

        bool isImplementResourceInterface = declaringType!
            .GetInterfaces()
            .Any(type => type.GetGenericTypeDefinition() == ResourceInterfaceType);
        
        // ReSharper disable once InvertIf
        if (propertyType.IsArray && isImplementResourceInterface)
        {
            var propertyResources = (propertyInfo.GetValue(@object) as object[])!;

            return propertyResources
                .Select(GetDescriptors)
                .ToArray();
        }
        
        throw new InvalidOperationException($"Cannot determine value type for '{declaringType.Name}'");
    }

    private static object? ConvertByteArrayToNumber(Type propertyType, object? value)
    {
        string? convertedValue;

        if (value is null)
        {
            convertedValue = null;
        }
        else
        {
            var castedValue = (byte[])value;
            convertedValue = Encoding.UTF8.GetString(castedValue);   
        }

        if (propertyType == Int16Type)
        {
            return short.Parse(convertedValue!);
        }
        
        if (propertyType == Int32Type)
        {
            return int.Parse(convertedValue!);
        }
        
        if (propertyType == Int64Type)
        {
            return long.Parse(convertedValue!);
        }
        
        if (propertyType == UInt16Type)
        {
            return ushort.Parse(convertedValue!);
        }
        
        if (propertyType == UInt32Type)
        {
            return uint.Parse(convertedValue!);
        }
        
        if (propertyType == UInt64Type)
        {
            return ulong.Parse(convertedValue!);
        }

        // ReSharper disable once InvertIf
        if (propertyType == NullableUInt64Type)
        {
            // ReSharper disable once InvertIf
            if (convertedValue is null)
            {
                ulong? valueToReturn = null;
                return valueToReturn;
            }
            
            return ulong.Parse(convertedValue);
        }

        throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType.Name);
    }
    
    private void SetValue(PropertyInfo propertyInfo, object @object, object? value)
    {
        Type? declaringType = propertyInfo.DeclaringType;
        Type propertyType = propertyInfo.PropertyType;

        if (propertyType.IsValueType && NumberTypes.Contains(propertyType))
        {
            object? convertedValue = ConvertByteArrayToNumber(propertyType, value);
            propertyInfo.SetValue(@object, convertedValue);
            return;
        }
        
        if (propertyType == StringType)
        {
            if (value is null)
            {
                propertyInfo.SetValue(@object, null);
                return;
            }
            
            var castedValue = (byte[])value;
            object convertedValue = Regex.Unescape(Encoding.Default.GetString(castedValue));
            propertyInfo.SetValue(@object, convertedValue);
            return;
        }

        if (propertyType == ByteType)
        {
            var castedValue = (byte[])value!;
            object convertedValue = byte.Parse(Encoding.UTF8.GetString(castedValue));
            propertyInfo.SetValue(@object, convertedValue);
            return;
        }
        
        if (propertyType == NullableByteType)
        {
            if (value is null)
            {
                propertyInfo.SetValue(@object, null);
                return;
            }
            
            var castedValue = (byte[])value;
            byte? convertedValue = byte.Parse(Encoding.UTF8.GetString(castedValue));
            propertyInfo.SetValue(@object, convertedValue);
            return;
        }

        if (propertyType == ByteArrayType)
        {
            if (value is null)
            {
                propertyInfo.SetValue(@object, null);
                return;
            }
            
            var castedValue = (byte[])value;
            byte[] convertedValue = Convert.FromBase64String(Encoding.UTF8.GetString(castedValue));
            propertyInfo.SetValue(@object, convertedValue);
            return;
        }

        if (propertyType == BooleanType)
        {
            propertyInfo.SetValue(@object, value);
            return;
        }

        bool isImplementResourceInterface = declaringType!
            .GetInterfaces()
            .Any(type => type.GetGenericTypeDefinition() == ResourceInterfaceType);
        
        // ReSharper disable once InvertIf
        if (propertyType.IsArray && isImplementResourceInterface)
        {
            var castedValues = (List<object>)value!;

            var arrayToSet = Array.CreateInstance(
                propertyType.GetElementType()!, castedValues.Count
            );

            for (var index = 0; index < castedValues.Count; index++)
            {
                object instance = Activator.CreateInstance(propertyType.GetElementType()!, true)!;
                var values = (Dictionary<string, object>)castedValues[index];
                SetDescriptors(instance, values);
                arrayToSet.SetValue(instance, index);
            }

            propertyInfo.SetValue(@object, arrayToSet);
            return;
        }
        
        throw new InvalidOperationException($"Cannot determine value type for '{declaringType.Name}'");
    }

    public static ManifestEntryDescriptors FromResource<TType>(IResourceType<TType> resource) where TType : IResourceType<TType>
    {
        return new ManifestEntryDescriptors
        {
            Descriptors = GetDescriptors(resource)
        };
    }

    public TType ToResource<TType>() where TType : IResourceType<TType>
    {
        var resource = (TType)Activator.CreateInstance(typeof(TType), true)!;
        SetDescriptors(resource);
        return resource;
    }

    public static ManifestEntryDescriptors CreateEmpty()
    {
        return new ManifestEntryDescriptors
        {
            Descriptors = new Dictionary<string, object>()
        };
    }

    public void AddFileName(string fileName)
    {
        Descriptors.Add("File", fileName);
    }

    public string GetFilename()
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!Descriptors.ContainsKey("File")) // TODO change key name
        {
            return null!;
        }
        
        return Encoding.UTF8.GetString((byte[])Descriptors["File"]);
    }
}