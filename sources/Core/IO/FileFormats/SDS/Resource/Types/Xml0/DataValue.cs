using System.Text;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Xml0;

internal class DataValue
{
    public DataType Type { get; }
    public object Value { get; }

    public DataValue(DataType type, object value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    internal static uint Serialize(Stream stream, DataValue? data, Endian endian)
    {
        var position = (uint)stream.Position;

        if (data is null)
        {
            return 0;
        }

        switch (data.Type)
        {
            case DataType.Special:
            {
                stream.WriteValueU32(1, endian);
                stream.WriteValueU32(0, endian);
                stream.WriteStringZ((string)data.Value, Encoding.UTF8);
                break;
            }

            case DataType.Boolean:
            {
                stream.WriteValueU32(2, endian);
                stream.WriteValueU32(0, endian);

                if (data.Value is bool value)
                {
                    stream.WriteValueU8(value ? (byte)1 : (byte)0);
                }
                else
                {
                    stream.WriteValueU8(bool.Parse((string)data.Value) ? (byte)1 : (byte)0);
                }

                break;
            }

            case DataType.Float:
            {
                stream.WriteValueU32(3, endian);
                stream.WriteValueU32(0, endian);

                if (data.Value is float value)
                {
                    stream.WriteValueF32(value, endian);
                }
                else
                {
                    stream.WriteValueF32(float.Parse((string)data.Value), endian);
                }

                break;
            }

            case DataType.String:
            {
                stream.WriteValueU32(4, endian);
                stream.WriteValueU32(0, endian);
                stream.WriteStringZ((string)data.Value, Encoding.UTF8);
                break;
            }

            case DataType.Integer:
            {
                stream.WriteValueU32(5, endian);
                stream.WriteValueU32(0, endian);

                if (data.Value is int value)
                {
                    stream.WriteValueS32(value, endian);
                }
                else
                {
                    stream.WriteValueS32(int.Parse((string)data.Value), endian);
                }

                break;
            }

            case DataType.Unknown:
            {
                stream.WriteValueU32(8, endian);
                stream.WriteValueU32(0, endian);
                stream.WriteValueU32(0, endian);
                stream.WriteValueU32(0, endian);

                stream.WriteValueF32(Convert.ToSingle(data.Value));

                break;
            }

            default:
            {
                throw new InvalidOperationException();
            }
        }

        return position;
    }

    internal static DataValue? Deserialize(Stream stream, uint offset, Endian endian)
    {
        stream.Seek(offset, SeekOrigin.Begin);

        var type = (DataType)stream.ReadValueU32(endian);

        switch (type)
        {
            case DataType.Special:
            {
                uint unk0 = stream.ReadValueU32(endian);

                if (unk0 != 0)
                {
                    throw new FormatException();
                }

                string value = stream.ReadStringZ(Encoding.UTF8);

                if (string.IsNullOrEmpty(value) || value.Contains("--") || value.Contains("\n\t >") || value.Contains("\t"))
                {
                    // Log.WriteLine("Detect unusual special: " + value); TODO log this
                    return null;
                }

                return new DataValue(type, value);
            }

            case DataType.Boolean:
            {
                uint unk0 = stream.ReadValueU32(endian);
                
                if (unk0 != 0)
                {
                    throw new FormatException();
                }

                bool value = stream.ReadValueU8() != 0;
                
                return new DataValue(type, value);
            }

            case DataType.Float:
            {
                uint unk0 = stream.ReadValueU32(endian);
                
                if (unk0 != 0)
                {
                    throw new FormatException();
                }

                float value = stream.ReadValueF32(endian);
                
                return new DataValue(type, value);
            }

            case DataType.String:
            {
                uint unk0 = stream.ReadValueU32(endian);

                if (unk0 != 0)
                {
                    throw new FormatException();
                }

                string value = stream.ReadStringZ(Encoding.UTF8);

                if (string.IsNullOrEmpty(value) || value.Contains("--"))
                {
                    return null;
                }

                return new DataValue(type, value);
            }

            case DataType.Integer:
            {
                uint unk0 = stream.ReadValueU32(endian);

                if (unk0 != 0)
                {
                    throw new FormatException();
                }

                int value = stream.ReadValueS32(endian);

                return new DataValue(type, value);
            }

            case DataType.Unknown:
            {
                uint unk0 = stream.ReadValueU32(endian);
                uint unk1 = stream.ReadValueU32(endian);
                uint unk2 = stream.ReadValueU32(endian);

                if (unk0 != 0 && unk1 != 0 && unk2 != 0)
                {
                    throw new InvalidDataException(
                        $"Unk data type is in invalid state => Unk0: {unk0}, Unk1: {unk1}, Unk2: {unk2}"
                    );
                }

                float value = stream.ReadValueF32(endian);
                
                return new DataValue(type, value);
            }

            default:
            {
                throw new FormatException();
            }
        }
    }
}