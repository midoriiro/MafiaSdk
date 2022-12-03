namespace Core.IO.ResourceFormats.XBin.Types.Cities;

public interface ICity
{
    static abstract ICity ReadFromFile(BinaryReader reader);
    void WriteToFile(XBinWriter writer);
}