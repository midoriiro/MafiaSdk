using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

// NOTE: Only supports M1: DE and M3 Cutscene formats.
// This IS NOT for Mafia II. However it would be ideal if we could also support Mafia II.
// TODO Mafia 2 support ?
public class CutsceneResource : IResourceType<CutsceneResource>
{
    [IgnoreFieldDescriptor]
    public int CutscenesCount => Cutscenes.Length;

    public CutsceneData[] Cutscenes { get; internal init; } = null!;

    private CutsceneResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32(0, endian);
        stream.WriteValueU32((uint)Cutscenes.Length, endian);

        foreach(CutsceneData cutsceneData in Cutscenes)
        {
            cutsceneData.Serialize(version, stream, endian);
        }
    }

    public static CutsceneResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        stream.ReadValueU32(endian); // padding
        uint count = stream.ReadValueU32(endian);

        var cutsceneData = new CutsceneData[count];

        for (var index = 0; index < cutsceneData.Length; index++)
        {
            cutsceneData[index] = CutsceneData.Deserialize(version, stream, endian);
        }

        return new CutsceneResource
        {
            Cutscenes = cutsceneData
        };
    }
}