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

internal readonly struct ResourceHeader
{
    public uint TypeId { get; init; }
    public uint Size { get; init; }
    public ushort Version { get; init; }
    public ulong FileHash { get; init; }
    public uint SlotRamRequired { get; init; }
    public uint SlotVramRequired { get; init; }
    public uint OtherRamRequired { get; init; }
    public uint OtherVramRequired { get; init; }

    public static ResourceHeader Read(Stream input, Endian endian, uint version)
    {
        var fileHash = 0UL;
            
        uint typeId = input.ReadValueU32(endian);
        uint size = input.ReadValueU32(endian);
        ushort resourceVersion = input.ReadValueU16(endian);
            
        if(version == 20)
        {
            fileHash = input.ReadValueU64(endian);
        }

        uint slotRamRequired = input.ReadValueU32(endian);
        uint slotVramRequired = input.ReadValueU32(endian);
        uint otherRamRequired = input.ReadValueU32(endian);
        uint otherVramRequired = input.ReadValueU32(endian);

        return new ResourceHeader
        {
            FileHash = fileHash,
            TypeId = typeId,
            Size = size,
            Version = resourceVersion,
            SlotRamRequired = slotRamRequired,
            SlotVramRequired = slotVramRequired,
            OtherRamRequired = otherRamRequired,
            OtherVramRequired = otherVramRequired
        };
    }

    public void Write(Stream output, Endian endian, uint version)
    {
        output.WriteValueU32(TypeId, endian);
        output.WriteValueU32(Size, endian);
        output.WriteValueU16(Version, endian);

        // Write exclusive version 20 data
        if(version == 20)
        {
            output.WriteValueU64(FileHash);
        }

        output.WriteValueU32(SlotRamRequired, endian);
        output.WriteValueU32(SlotVramRequired, endian);
        output.WriteValueU32(OtherRamRequired, endian);
        output.WriteValueU32(OtherVramRequired, endian);
    }
}