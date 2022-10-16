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

namespace Core.IO.FileFormats.SDS.Archive
{
    public struct ResourceType : IEquatable<ResourceType>
    {
        public uint Id;
        public string Name;
        public uint Parent;

        public static ResourceType Read(Stream input, Endian endian)
        {
            ResourceType instance;
            instance.Id = input.ReadValueU32(endian);
            instance.Name = input.ReadStringU32(endian);
            instance.Parent = input.ReadValueU32(endian);
            return instance;
        }
        
        public void Write(Stream output, Endian endian)
        {
            output.WriteValueU32(Id, endian);
            output.WriteStringU32(Name, endian);
            output.WriteValueU32(Parent, endian);
        }

        public bool Equals(ResourceType other)
        {
            return Id == other.Id &&
                   string.Equals(Name, other.Name) &&
                   Parent == other.Parent;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ResourceType type && Equals(type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Parent;
                return hashCode;
            }
        }

        public static bool operator ==(ResourceType left, ResourceType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceType left, ResourceType right)
        {
            return left.Equals(right) == false;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Parent: {Parent}";
        }
    }
}
