using Core.IO.ResourceFormats.Havok.Classes;
using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public static class ClassFactory
{
    public static IClass Deserialize(string className, Stream stream, Endian endian, Header header, Object @object)
    {
        return className switch
        {
            "hkRootLevelContainer" => RootLevelContainer.Deserialize(stream, endian, header, @object),
            "hkaAnimationContainer" => AnimationContainer.Deserialize(stream, endian, header, @object),
            "hkaSplineCompressedAnimation" => SplineCompressedAnimation.Deserialize(stream, endian, header, @object),
            "hkaDefaultAnimatedReferenceFrame" => DefaultAnimationReferenceFrame.Deserialize(stream, endian, header, @object),
            "hkaAnimationBinding" => AnimationBinding.Deserialize(stream, endian, header, @object),
            "hkaSkeleton" => Skeleton.Deserialize(stream, endian, header, @object),
            "hkaInterleavedUncompressedAnimation" => InterleavedUncompressedAnimation.Deserialize(stream, endian, header, @object),
            "hkaSkeletonMapper" => SkeletonMapper.Deserialize(stream, endian, header, @object),
            "hkaiNavMesh" => NavMesh.Deserialize(stream, endian, header, @object),
            "hkaiDirectedGraphExplicitCost" => DirectedGraphExplicitCost.Deserialize(stream, endian, header, @object),
            "hkcdStaticAabbTree" => StaticAabbTree.Deserialize(stream, endian, header, @object),
            "hkaiNavMeshClearanceCacheSeeder" => NavMeshClearanceCacheSeeder.Deserialize(stream, endian, header, @object),
            "hkcdStaticTreeDefaultTreeStorage6" => StaticTreeDefaultTreeStorage6.Deserialize(stream, endian, header, @object),
            "hkaiNavMeshClearanceCache" => NavMeshClearanceCache.Deserialize(stream, endian, header, @object),
            _ => throw new ArgumentOutOfRangeException(nameof(className), $"Unknown class name '{className}'")
        };
    }
}