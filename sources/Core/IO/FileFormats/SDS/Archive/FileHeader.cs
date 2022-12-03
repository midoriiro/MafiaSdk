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

using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Archive;

public readonly struct FileHeader
{
    public uint ResourceTypeTableOffset { get; init; }
    public uint BlockTableOffset { get; init; }
    public uint XmlOffset { get; init; }
    public uint SlotRamRequired { get; init; }
    public uint SlotVramRequired { get; init; }
    public uint OtherRamRequired { get; init; }
    public uint OtherVramRequired { get; init; }
    public uint Flags { get; init; }
    public byte[] Unknown20 { get; init; }
    public uint ResourceCount { get; init; }

    public static FileHeader Read(Stream input, Endian endian)
    {
        uint resourceTypeTableOffset = input.ReadValueU32(endian);
        uint blockTableOffset = input.ReadValueU32(endian);
        uint xmlOffset = input.ReadValueU32(endian);
        uint slotRamRequired = input.ReadValueU32(endian);
        uint slotVramRequired = input.ReadValueU32(endian);
        uint otherRamRequired = input.ReadValueU32(endian);
        uint otherVramRequired = input.ReadValueU32(endian);
        uint flags = input.ReadValueU32(endian);
        byte[] unknown20 = input.ReadBytes(16);
        uint resourceCount = input.ReadValueU32(endian);

        return new FileHeader
        {
            ResourceTypeTableOffset = resourceTypeTableOffset,
            BlockTableOffset = blockTableOffset,
            XmlOffset = xmlOffset,
            SlotRamRequired = slotRamRequired,
            SlotVramRequired = slotVramRequired,
            OtherRamRequired = otherRamRequired,
            OtherVramRequired = otherVramRequired,
            Flags = flags,
            Unknown20 = unknown20,
            ResourceCount = resourceCount
        };
    }

    public void Write(Stream output, Endian endian)
    {
        output.WriteValueU32(ResourceTypeTableOffset, endian);
        output.WriteValueU32(BlockTableOffset, endian);
        output.WriteValueU32(XmlOffset, endian);
        output.WriteValueU32(SlotRamRequired, endian);
        output.WriteValueU32(SlotVramRequired, endian);
        output.WriteValueU32(OtherRamRequired, endian);
        output.WriteValueU32(OtherVramRequired, endian);
        output.WriteValueU32(Flags, endian);
        output.Write(Unknown20, 0, Unknown20.Length);
        output.WriteValueU32(ResourceCount, endian);
    }
}