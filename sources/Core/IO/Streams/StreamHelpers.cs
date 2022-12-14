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
    private static readonly Dictionary<Stream, List<long>> Steps = new ();

    internal static bool ShouldSwap(Endian endian)
    {
        return endian switch
        {
            Endian.Little => BitConverter.IsLittleEndian == false,
            // ReSharper disable once RedundantBoolCompare
            Endian.Big => BitConverter.IsLittleEndian == true,
            _ => throw new ArgumentException("Unsupported endianness", nameof(endian))
        };
    }

    public static MemoryStream ReadToMemoryStream(this Stream stream, int size, bool writeable)
    {
        byte[] bytes = stream.ReadBytes(size);
        return new MemoryStream(bytes, writeable);
    }

    public static MemoryStream ReadToMemoryStream(this Stream stream, int size)
    {
        return stream.ReadToMemoryStream(size, true);
    }

    public static void WriteFromStream(this Stream stream, Stream input, long size, int buffer)
    {
        long left = size;
        var data = new byte[buffer];
        while (left > 0)
        {
            var block = (int)Math.Min(left, buffer);
            int read = input.Read(data, 0, block);
            if (read != block)
            {
                throw new EndOfStreamException();
            }
            stream.Write(data, 0, block);
            left -= block;
        }
    }

    public static void WriteFromStream(this Stream stream, Stream input, long size)
    {
        stream.WriteFromStream(input, size, 0x40000);
    }

    public static byte[] ReadBytes(this Stream stream, int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var data = new byte[length];
        int read = stream.Read(data, 0, length);
        if (read != length)
        {
            throw new EndOfStreamException();
        }
        return data;
    }

    public static void WriteBytes(this Stream stream, byte[] data)
    {
        stream.Write(data, 0, data.Length);
    }

    public static void StepIn(this Stream stream, long offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        if (!Steps.ContainsKey(stream))
        {
            Steps.Add(stream, new List<long>());
        }
        
        Steps[stream].Add(stream.Position);
        stream.Seek(offset, origin);
    }

    public static void StepOut(this Stream stream)
    {
        if (!Steps.ContainsKey(stream))
        {
            throw new InvalidOperationException("Cannot step out from a stream that never step in");
        }

        long step = Steps[stream].TakeLast(1).Single();
        stream.Seek(step, SeekOrigin.Begin);
    }

    public static byte[] Peek(this Stream stream, int length)
    {
        if (length <= 1)
        {
            throw new ArgumentException("Cannot be equal to 1 or below 0", nameof(length));
        }
        
        byte[] value = stream.ReadBytes(length);
        stream.Seek(-length, SeekOrigin.Current);
        return value;
    }
    
    public static int Peek(this Stream stream)
    {
        int value = stream.ReadByte();
        stream.Seek(-1, SeekOrigin.Current);
        return value;
    }

    public static void AlignTo(this Stream stream, uint alignment)
    {
        if (alignment <= 0)
        {
            throw new ArgumentException("Cannot align when distance is equal or below 0", nameof(alignment));
        }

        long distance = alignment - stream.Position % alignment;

        if (distance != alignment)
        {
            stream.Seek(distance, SeekOrigin.Current);
        }
    }
}