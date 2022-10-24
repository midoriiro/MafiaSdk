namespace Core.Games;

public class GameWorkSpace
{
    private static GameWorkSpace? _instance;

    private string _backupFolderPath;
    
    public Game SelectedGame { get; }
    
    public string Path { get; }

    private GameWorkSpace(
        GamesEnumerator selectedGame, 
        string installationPath,
        string workSpacePath
    )
    {
        SelectedGame = new Game(installationPath, selectedGame);
        string gameWorkSpacePath = System.IO.Path.Join(
            System.IO.Path.GetFullPath(workSpacePath), 
            GetGameSubfolder(selectedGame)
        );
        EnsurePathExists(gameWorkSpacePath);
        Path = gameWorkSpacePath;
        _backupFolderPath = System.IO.Path.Join(Path, ".backup");
    }
    
    public static GameWorkSpace Create(
        GamesEnumerator selectedGame,
        string installationPath,
        string workSpacePath
    )
    {
        return _instance ??= new GameWorkSpace(selectedGame, installationPath, workSpacePath);
    }

    public static GameWorkSpace Instance()
    {
        return _instance!;
    }
    
    private static string GetGameSubfolder(GamesEnumerator type)
    {
        return type switch
        {
            GamesEnumerator.Mafia1DefinitiveEdition => "mafia-definitive-edition",
            GamesEnumerator.Mafia2 => "mafia2",
            GamesEnumerator.Mafia2DefinitiveEdition => "mafia2-definitive-edition",
            GamesEnumerator.Mafia3 => "mafia3",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "No game found")
        };
    }

    private static void EnsurePathExists(string path)
    {
        Directory.CreateDirectory(path);
    }

    public string GetExtractPath(string path)
    {
        string installationPath = SelectedGame.InstallationPath;
        
        if (!path.StartsWith(installationPath))
        {
            throw new ArgumentException($"Path should start with '{installationPath}'", nameof(path));
        }

        return path.Replace(installationPath, Path);
    }
    
    public string GetBackupPath(string path)
    {
        string installationPath = SelectedGame.InstallationPath;
        
        if (!path.StartsWith(installationPath))
        {
            throw new ArgumentException($"Path should start with '{installationPath}'", nameof(path));
        }

        return path.Replace(installationPath, _backupFolderPath);
    }
}