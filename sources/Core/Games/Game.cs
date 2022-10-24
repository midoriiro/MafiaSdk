namespace Core.Games;

public class Game
{
    public string Name { get; }
    public string InstallationPath { get; }
    public GamesEnumerator Type { get; }

    public Game(string installationPath, GamesEnumerator type)
    {
        EnsureInstallationPathExists(installationPath, type);
        Name = GetName(type);
        InstallationPath = installationPath;
        Type = type;
    }

    private static string GetName(GamesEnumerator type)
    {
        return type switch
        {
            GamesEnumerator.Mafia1DefinitiveEdition => "Mafia Definition Edition",
            GamesEnumerator.Mafia2 => "Mafia 2",
            GamesEnumerator.Mafia2DefinitiveEdition => "Mafia 2 Definitive Edition",
            GamesEnumerator.Mafia3 => "Mafia 3",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "No game found")
        };
    }
    
    private static string GetExecutableName(GamesEnumerator type)
    {
        return type switch
        {
            GamesEnumerator.Mafia1DefinitiveEdition => "mafiadefinitiveedition.exe",
            GamesEnumerator.Mafia2 => "mafia2.exe",
            GamesEnumerator.Mafia2DefinitiveEdition => "mafia ii definitive edition.exe",
            GamesEnumerator.Mafia3 => "mafia3definitiveedition.exe",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "No game found")
        };
    }

    private static void EnsureInstallationPathExists(string installationPath, GamesEnumerator selectedGame)
    {
        string executableName = GetExecutableName(selectedGame);
        string filePath = Path.Join(installationPath, executableName);
        
        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException(
                $"Installation path '{installationPath}' is not containing executable file '{executableName}'"
            );
        }
    }
}