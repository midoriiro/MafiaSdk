namespace Core.IO.ResourceFormats.XBin.Types.Vehicles
{
    public interface IVehicleTableItem
    {
        static abstract IVehicleTableItem ReadEntry(BinaryReader reader);
        void WriteEntry(XBinWriter writer);
    }
}
