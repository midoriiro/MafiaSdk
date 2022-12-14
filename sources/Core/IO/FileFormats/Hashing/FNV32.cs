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

using System.Text;

namespace Core.IO.FileFormats.Hashing;

public static class Fnv32
{
    public const uint Initial = 0x811C9DC5;

    public static uint Hash(string value)
    {
        return Hash(value, Encoding.UTF8);
    }

    public static uint Hash(string? value, Encoding encoding)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (value is null)
        {
            return Hash(null, 0, 0);
        }

        byte[] bytes = encoding.GetBytes(value);
        return Hash(bytes, 0, bytes.Length);
    }

    public static uint Hash(byte[]? buffer, int offset, int count, uint hash = Initial)
    {
        if (buffer == null)
        {
            return hash;
        }

        for (int i = offset; i < offset + count; i++)
        {
            hash *= 0x1000193;
            hash ^= buffer[i];
        }

        return hash;
    }
}