/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Core.IO.Streams;

public static partial class StreamHelpers
{
    #region Cache
    private static class EnumTypeCache
    {
        /*private static Dictionary<Type, EnumUnderlyingType> _Lookup;

        static EnumTypeCache()
        {
            _Lookup = new Dictionary<Type, EnumUnderlyingType>();
        }*/

        private static TypeCode TranslateType(Type type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Unknown enum type", nameof(type));
            }

            Type underlyingType = Enum.GetUnderlyingType(type);
            TypeCode underlyingTypeCode = Type.GetTypeCode(underlyingType);

            switch (underlyingTypeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                {
                    return underlyingTypeCode;
                }
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static TypeCode Get(Type type)
        {
            /*if (Lookup.ContainsKey(type) == true)
            {
                return Lookup[type];
            }*/

            return /*Lookup[type] =*/ TranslateType(type);
        }
    }
    #endregion

    #region ReadValueEnum
    public static T ReadValueEnum<T>(this Stream stream, Endian endian)
    {
        Type type = typeof(T);

        object value = EnumTypeCache.Get(type) switch
        {
            TypeCode.SByte => stream.ReadValueS8(),
            TypeCode.Byte => stream.ReadValueU8(),
            TypeCode.Int16 => stream.ReadValueS16(endian),
            TypeCode.UInt16 => stream.ReadValueU16(endian),
            TypeCode.Int32 => stream.ReadValueS32(endian),
            TypeCode.UInt32 => stream.ReadValueU32(endian),
            TypeCode.Int64 => stream.ReadValueS64(endian),
            TypeCode.UInt64 => stream.ReadValueU64(endian),
            _ => throw new NotSupportedException()
        };

        return (T)Enum.ToObject(type, value);
    }

    public static T ReadValueEnum<T>(this Stream stream)
    {
        return stream.ReadValueEnum<T>(Endian.Little);
    }
    #endregion

    #region WriteValueEnum
    public static void WriteValueEnum<T>(this Stream stream, object value, Endian endian)
    {
        Type type = typeof(T);
        switch (EnumTypeCache.Get(type))
        {
            case TypeCode.SByte:
            {
                stream.WriteValueS8((sbyte)value);
                break;
            }

            case TypeCode.Byte:
            {
                stream.WriteValueU8((byte)value);
                break;
            }

            case TypeCode.Int16:
            {
                stream.WriteValueS16((short)value, endian);
                break;
            }

            case TypeCode.UInt16:
            {
                stream.WriteValueU16((ushort)value, endian);
                break;
            }

            case TypeCode.Int32:
            {
                stream.WriteValueS32((int)value, endian);
                break;
            }

            case TypeCode.UInt32:
            {
                stream.WriteValueU32((uint)value, endian);
                break;
            }

            case TypeCode.Int64:
            {
                stream.WriteValueS64((long)value, endian);
                break;
            }

            case TypeCode.UInt64:
            {
                stream.WriteValueU64((ulong)value, endian);
                break;
            }

            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
            {
                throw new NotSupportedException();
            }
        }
    }

    public static void WriteValueEnum<T>(this Stream stream, object value)
    {
        stream.WriteValueEnum<T>(value, Endian.Little);
    }
    #endregion
}