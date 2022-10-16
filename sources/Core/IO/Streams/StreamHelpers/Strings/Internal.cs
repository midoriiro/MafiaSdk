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

using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Core.IO.Streams;

public static partial class StreamHelpers
{
    internal static string ReadStringInternalStatic(
        this Stream stream,
        Encoding encoding,
        int size,
        bool trailingNull
    )
    {
        byte[] data = stream.ReadBytes(size);
        string value = encoding.GetString(data, 0, data.Length);

        if (!trailingNull)
        {
            return value;
        }

        int position = value.IndexOf('\0');
        if (position >= 0)
        {
            value = value[..position];
        }

        return value;
    }

    internal static void WriteStringInternalStatic(this Stream stream, Encoding encoding, string value)
    {
        byte[] data = encoding.GetBytes(value);
        stream.Write(data, 0, data.Length);
    }

    internal static void WriteStringInternalStatic(this Stream stream, Encoding encoding, string value, int size)
    {
        byte[] data = encoding.GetBytes(value);
        Array.Resize(ref data, size);
        stream.Write(data, 0, size);
    }

    internal static string ReadStringInternalDynamic(this Stream stream, Encoding encoding, char end)
    {
        int characterSize = encoding.GetByteCount("e");
        Debug.Assert(characterSize is 1 or 2 or 4);
        var characterEnd = end.ToString(CultureInfo.InvariantCulture);

        var i = 0;
        var data = new byte[128 * characterSize];

        while (true)
        {
            if (i + characterSize > data.Length)
            {
                Array.Resize(ref data, data.Length + 128 * characterSize);
            }

            int read = stream.Read(data, i, characterSize);
            Debug.Assert(read == characterSize);

            if (encoding.GetString(data, i, characterSize) == characterEnd)
            {
                break;
            }

            i += characterSize;
        }

        return i == 0 ? string.Empty : encoding.GetString(data, 0, i);
    }

    internal static void WriteStringInternalDynamic(this Stream stream, Encoding encoding, string value, char end)
    {
        byte[] data = encoding.GetBytes(value);
        stream.Write(data, 0, data.Length);

        data = encoding.GetBytes(end.ToString(CultureInfo.InvariantCulture));
        stream.Write(data, 0, data.Length);
    }
}