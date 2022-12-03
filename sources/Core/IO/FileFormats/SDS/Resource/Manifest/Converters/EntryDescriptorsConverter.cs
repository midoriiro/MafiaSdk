using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.IO.FileFormats.SDS.Resource.Manifest.Converters;

public class EntryDescriptorsConverter : JsonConverter<ManifestEntryDescriptors>
{
    private static object ReadValue(ref Utf8JsonReader reader, bool readValue = true)
    {
        if (readValue)
        {
            reader.Read();
        }

        switch (reader.TokenType)
        {
            case JsonTokenType.StartArray:
            {
                List<object> array = new();

                while (true)
                {
                    reader.Read();
                
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }
                
                    object value = ReadValue(ref reader, false);
                    array.Add(value);
                }

                return array;
            }
            case JsonTokenType.StartObject:
            {
                Dictionary<string, object> @object = new();
            
                while (true)
                {
                    reader.Read();
                
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }
                
                    string key = reader.GetString()!;
                
                    reader.Read();
                    object value = ReadValue(ref reader, false);
                    @object.Add(key, value);
                }

                return @object;
            }
            case JsonTokenType.False or JsonTokenType.True:
                return reader.GetBoolean();
            case JsonTokenType.Null:
                return null!;
        }

        return reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
    }
    
    public override ManifestEntryDescriptors Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var descriptors = new Dictionary<string, object>();

        while (true)
        {
            reader.Read();
            
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }
            
            string key = reader.GetString()!;
            object value = ReadValue(ref reader);
            
            descriptors.Add(key, value);
        }

        return new ManifestEntryDescriptors
        {
            Descriptors = descriptors
        };
    }

    public override void Write(Utf8JsonWriter writer, ManifestEntryDescriptors value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var descriptor in value.Descriptors)
        {
            writer.WritePropertyName(descriptor.Key);
            JsonSerializer.Serialize(writer, descriptor.Value, descriptor.Value.GetType(), options);
        }
        
        writer.WriteEndObject();
    }
}