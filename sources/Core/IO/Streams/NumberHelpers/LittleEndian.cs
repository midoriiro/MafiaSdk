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

public static partial class NumberHelpers
{
    public static short LittleEndian(this short value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static ushort LittleEndian(this ushort value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static int LittleEndian(this int value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static uint LittleEndian(this uint value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static long LittleEndian(this long value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static ulong LittleEndian(this ulong value)
    {
        return BitConverter.IsLittleEndian == false ? value.Swap() : value;
    }

    public static float LittleEndian(this float value)
    {
        if (!BitConverter.IsLittleEndian)
        {
            return value;
        }

        byte[] data = BitConverter.GetBytes(value);
        uint junk = BitConverter.ToUInt32(data, 0).Swap();
        return BitConverter.ToSingle(BitConverter.GetBytes(junk), 0);
    }

    public static double LittleEndian(this double value)
    {
        if (BitConverter.IsLittleEndian != true)
        {
            return value;
        }

        byte[] data = BitConverter.GetBytes(value);
        ulong junk = BitConverter.ToUInt64(data, 0).Swap();
        return BitConverter.ToDouble(BitConverter.GetBytes(junk), 0);
    }
}