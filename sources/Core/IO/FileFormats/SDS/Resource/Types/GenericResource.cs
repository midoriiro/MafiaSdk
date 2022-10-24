using Core.IO.FileFormats.Hashing;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class GenericResource : IResourceType<GenericResource>
{
    private readonly Dictionary<ulong, string> _typeExtensionMagic = new Dictionary<ulong, string>()
    {
        { 0x15B770C22,  ".vi.compiled" },
        { 0xA53038C9,  ".flownode" },
        { 0xC3A9C338,  ".dlgsel" },
        { 0x164D0E75C,  ".ires.compiled" },
        { 0xA757FB5364D0E75C,  ".ires.[nomesh].compiled" },
        { 0x222FDF7264D0E75C,  ".ires.[lod0].compiled" },
        { 0x222FDF7364D0E75C,  ".ires.[lod1].compiled" },
        { 0x222FDF7064D0E75C,  ".ires.[lod2].compiled" },
        { 0x1B4347D18,   ".entity.compiled" },
        { 0x4DE17E9B,  ".gbin" },
        { 0x45F07C8B,  ".gxml" },
        { 0x172D9EA8F, ".scene.compiled" },
        { 0x16AD0740B, ".collision.compiled" },
        { 0xA757FB5372D9EA8F,  ".scene.[nomesh].compiled" },
        { 0x222FDF7272D9EA8F,  ".scene.[lod0].compiled" },
        { 0x222FDF7372D9EA8F,  ".scene.[lod1].compiled" },
        { 0x222FDF7072D9EA8F,  ".scene.[lod2].compiled" },
        { 0x1E5CA8123,  ".streaming.compiled" },
        { 0x16024B481,  ".bpdb.compiled" },
        { 0x57572DAC256CA1DB,  ".trb.[global].compiled" },
        { 0x1256CA1DB, ".trb.compiled" },
        { 0xA27F694D, ".iprofai" },
        { 0x4A336D64, ".iproftime" },
        { 0x222FDF72B2F3E413, ".lodbaked.[lod0].compiled" },
        { 0x222FDF73B2F3E413, ".lodbaked.[lod1].compiled" },
        { 0x222FDF70B2F3E413, ".lodbaked.[lod2].compiled" },
        { 0xF3BB3621, ".ccdb" },
        { 0x28930A39, ".egr" },
        { 0x1D4AD8D9E, ".cegr.compiled" },
        { 0x428F61D4, ".fmv.compiled" }, // NOTE: Type is a guess, used in cine_ in M1: DE.
        { 0x118E35C27, ".animprojectreflected.compiled" },
        { 0x73CB32C9, ".effects" }
        //{ 0x45F07C8B, ".scene.gxml"  },
    };

    private readonly Dictionary<string, ulong> _typeExtensionString = new Dictionary<string, ulong>()
    {
        { ".vi.compiled", 0x15B770C22 },
        { ".flownode", 0xA53038C9 },
        { ".dlgsel", 0xC3A9C338 },
        { ".ires.compiled", 0x164D0E75C },
        { ".ires.[nomesh].compiled", 0xA757FB5364D0E75C },
        { ".ires.[lod0].compiled", 0x222FDF7264D0E75C },
        { ".ires.[lod1].compiled", 0x222FDF7364D0E75C },
        { ".ires.[lod2].compiled", 0x222FDF7064D0E75C },
        { ".entity.compiled", 0x1B4347D18 },
        { ".gbin", 0x4DE17E9B },
        { ".gxml", 0x45F07C8B },
        { ".scene.compiled", 0x172D9EA8F },
        { ".collision.compiled", 0x16AD0740B },
        { ".scene.[nomesh].compiled", 0xA757FB5372D9EA8F },
        { ".scene.[lod0].compiled", 0x222FDF7272D9EA8F },
        { ".scene.[lod1].compiled", 0x222FDF7372D9EA8F },
        { ".scene.[lod2].compiled", 0x222FDF7072D9EA8F },
        { ".streaming.compiled", 0x1E5CA8123 },
        { ".bpdb.compiled", 0x16024B481 },
        { ".trb.[global].compiled", 0x57572DAC256CA1DB },
        { ".trb.compiled", 0x1256CA1DB },
        { ".iprofai", 0xA27F694D },
        { ".iproftime", 0x4A336D64 },
        { ".fmv", 0x428F61D4 },
        { ".lodbaked.[lod0].compiled", 0x222FDF72B2F3E413 },
        { ".lodbaked.[lod1].compiled", 0x222FDF73B2F3E413 },
        { ".lodbaked.[lod2].compiled", 0x222FDF70B2F3E413 },
        { ".ccdb", 0xF3BB3621 },
        { ".egr", 0x28930A39 },
        { ".cegr.compiled", 0x1D4AD8D9E },
        { ".fmv.compiled", 0x428F61D4 }, // NOTE: Type is a guess, used in cine_ in M1: DE.
        { ".animprojectreflected.compiled",  0x118E35C27},
        { ".effects", 0x73CB32C9 }
        //{ ".scene.gxml", 0x45F07C8B  }
    };
    
    public ulong GenericType { get; set; }
    public ushort Unk0 { get; set; }
    public string DebugName { get; set; } = null!;
    public byte[] Data { get; set; } = null!;

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        GenericType = DetermineMagic(DebugName);

        string tempName = string.IsNullOrEmpty(DebugName) ? "" : DebugName;

        stream.WriteValueU64(GenericType);
        stream.WriteValueU16(Unk0);
        stream.WriteStringU16(tempName, endian);
        stream.WriteBytes(Data);
    }

    public static GenericResource Deserialize(ushort version, Stream input, Endian endian)
    {
        ulong genericType = input.ReadValueU64();
        ushort unk0 = input.ReadValueU16();
        string debugName = input.ReadStringU16(endian);

        // We do not have any size so we do (FILE_LENGTH - CURRENT_POS);
        byte[] data = input.ReadBytes((int)(input.Length - input.Position));

        return new GenericResource()
        {
            GenericType = genericType,
            Unk0 = unk0,
            DebugName = debugName,
            Data = data
        };
    }

    public ulong DetermineMagic(string name)
    {
        string extension = GetFullExtensionUtil(name);
        ulong magic = 0;

        bool hasFound = _typeExtensionString.ContainsKey(extension);

        if(!hasFound)
        {
            hasFound = RecursiveExtensionCheck(ref extension);
        }

        if(hasFound)
        {
            magic = _typeExtensionString[extension];
        }
        else
        {
            // TODO log this
            // MessageBox.Show("Detected an unknown extension!!! SDS will NOT work!", "Toolkit");
        }

        return magic;
    }

    public string DetermineName(ResourceEntry entry, string name)
    {
        var gotDebugName = false;

        // Make sure we use the debug name.
        if (!string.IsNullOrEmpty(DebugName))
        {
            name = DebugName;
            gotDebugName = true;
        }

        // We found the name already
        if(FNV64.Hash(name) == entry.FileHash)
        {
            name = ReplaceCurrentExtension(name);
            return name;
        }

        // Our database tool has figured out this file name.
        // Return.
        // TODO: Consider an easier approach for this, maybe have a flag?
        if (!name.Contains("File_") && !gotDebugName)
        {
            string extension = GetFullExtensionUtil(name);
            if(!_typeExtensionString.ContainsKey(extension))
            {
                // TODO log this
                // MessageBox.Show("Detected missing extension from DB. Please contract Greavesy with SDS name.", "Toolkit");
            }
            return name;
        }

        if (!gotDebugName)
        {
            string withoutExtension = Path.GetFileNameWithoutExtension(name);

            if(_typeExtensionMagic.ContainsKey(GenericType))
            {
                string extension = _typeExtensionMagic[GenericType];
                withoutExtension += extension;
            }
            else 
            {
                withoutExtension += ".genr";
                // TODO log this
                // MessageBox.Show("Detected an unknown GENR type. Please contract Greavesy with SDS name.", "Toolkit");
            }

            name = withoutExtension;
        }

        return name;
    }

    private bool RecursiveExtensionCheck(ref string extension)
    {
        while(extension.LastIndexOf('.') != 0)
        {
            string removedDot = extension.Remove(0, 1);
            extension = GetFullExtensionUtil(removedDot);

            bool hasFound = _typeExtensionString.ContainsKey(extension);
            
            if(hasFound)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetFullExtensionUtil(string fileName)
    {
        int extensionStart = fileName.IndexOf(".", StringComparison.Ordinal);
        return fileName[extensionStart..];
    }

    private string ReplaceCurrentExtension(string fileName)
    {
        string detectedExtension = _typeExtensionMagic[GenericType];
        string extension = GetFullExtensionUtil(fileName);
        int indexOfExtension = fileName.IndexOf(extension, StringComparison.Ordinal);

        if (indexOfExtension == -1)
        {
            return fileName;
        }

        string newName = fileName.Remove(indexOfExtension);
        newName += detectedExtension;
        return newName;
    }
}