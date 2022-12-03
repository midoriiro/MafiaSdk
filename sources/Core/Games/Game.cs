using Core.IO.Files.Scanner;

namespace Core.Games;

public class Game
{
    private static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };
    
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
            GamesEnumerator.Mafia2DefinitiveEdition => "Mafia II Definitive Edition.exe",
            GamesEnumerator.Mafia3 => "Mafia3DefinitiveEdition.exe",
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

    public static string FindInstallationPath(DriveInfo[] driveInfos, GamesEnumerator selectedGame)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        string installationPath = null!;
        
        Parallel.For(0, driveInfos.Length, ParallelOptions, index =>
        {
            DriveInfo driveInfo = driveInfos[index];

            string? path = FindInstallationPath(driveInfo, selectedGame, cancellationToken);

            if (path is null)
            {
                return;
            }

            installationPath = path;
            cancellationTokenSource.Cancel();
        });

        if (installationPath is null)
        {
            throw new InvalidOperationException("Cannot determine installation path");
        }

        return installationPath;
    }

    private static string? FindInstallationPath(
        DriveInfo driveInfo, 
        GamesEnumerator selectedGame, 
        CancellationToken? cancellationToken = null
    )
    {
        string executableName = GetExecutableName(selectedGame);
        string rootPath = driveInfo.RootDirectory.FullName;
        string? filePath = FileScanner.ScanSingle(rootPath, executableName, cancellationToken);
        return filePath is null ? null : Path.GetDirectoryName(filePath)!;
    }
}