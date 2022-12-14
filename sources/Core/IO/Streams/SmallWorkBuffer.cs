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

internal static class SmallWorkBuffer
{
    private const int BufferSize = 8;

    private static readonly ThreadLocal<byte[]> Buffer = new(() => new byte[BufferSize]);

    public static byte[] Get(int count)
    {
        if (count is < 0 or > BufferSize)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        return Buffer.Value!;
    }

    public static byte[] ReadBytes(Stream stream, int count)
    {
        if (count is < 0 or > BufferSize)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        
        byte[] buffer = Buffer.Value!;
        int read = stream.Read(buffer, 0, count);
        
        if (read != count)
        {
            throw new EndOfStreamException();
        }
        
        return buffer;
    }
}