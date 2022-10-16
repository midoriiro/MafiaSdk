namespace Core.IO.FileFormats.SDS;

internal struct ArchiveEncryptionSettings
{
    public readonly uint Sum;
    public readonly uint Delta;
    public readonly uint Rounds;

    public ArchiveEncryptionSettings(uint sum, uint delta, uint rounds)
    {
        Sum = sum;
        Delta = delta;
        Rounds = rounds;
    }
}