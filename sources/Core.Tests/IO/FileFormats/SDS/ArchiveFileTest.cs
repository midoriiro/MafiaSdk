using System.Diagnostics;
using System.Reflection;
using Core.Games;
using Core.IO.FileFormats.SDS;
using Core.IO.FileFormats.SDS.Resource;
using Core.IO.FileFormats.SDS.Resource.Streams;
using Core.IO.FileFormats.SDS.Resource.Streams.Logging;
using Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;
using Core.IO.FileFormats.SDS.Resource.Streams.Strategies;
using Core.IO.FileFormats.SDS.Resource.Streams.Writers;
using Core.IO.Files;
using Core.IO.Files.Scanner;
using Core.IO.Streams;
using Core.Tests.IO.FileFormats.SDS;
using Core.Tests.Utils;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Core.Tests.FileFormats.Mafia3;

public class ArchiveFileTest
{
    private static DataWriterScheduler CreateDataWriterScheduler(Action<DataWriterMonitor> setMonitor)
    {
        var dataWriterMonitor = new DataWriterMonitor();
        setMonitor(dataWriterMonitor);
        
        var dataWriter = new FileDataWriter();
        // var dataWriterStrategy = new MemoryThresholdDataWriterStrategy()
        // {
        // TriggerAt = MemorySize.From(1, MemoryUnit.Giga)
        // };
        var dataWriterStrategy = new SequentialDataWriterStrategy();
        return new DataWriterScheduler(dataWriterStrategy, dataWriter, dataWriterMonitor);
    }

    private static string GetCheckPointFilePath()
    {
        var stackTrace = new StackTrace();
        StackFrame stackFrame = stackTrace.GetFrame(2)!;
        string methodName = stackFrame.GetMethod()!.Name;

        string fileName = methodName + ".checkpoint";

        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string path = Path.GetDirectoryName(assemblyLocation)!;

        return Path.Join(path, fileName);
    }

    private static int LoadCheckPoint()
    {
        string filePath = GetCheckPointFilePath();

        return File.Exists(filePath) ? int.Parse(File.ReadAllText(filePath)) : 0;
    }

    private static void SaveCheckPoint(int index)
    {
        string filePath = GetCheckPointFilePath();
        File.WriteAllText(filePath, index.ToString());
    }

    [InlineData(GamesEnumerator.Mafia1DefinitiveEdition, true)]
    [InlineData(GamesEnumerator.Mafia2, true)]
    [InlineData(GamesEnumerator.Mafia2DefinitiveEdition, true)]
    [InlineData(GamesEnumerator.Mafia3, true)]
    [Theory]
    public void DecompressAndVerify(GamesEnumerator selectedGame, bool useCheckPoint)
    {
        GameWorkSpace workspace = GameWorkSpace.Find(
            selectedGame,
            @"E:\mafia-sdk-workspaces"
        );

        NullDataWriterLogger dataWriterLogger = null!;
        DataWriterScheduler dataWriterScheduler = CreateDataWriterScheduler(
            monitor => dataWriterLogger = new NullDataWriterLogger(monitor)
        );
        
        var files = FileScanner.ScanByExtension<SdsFile>(workspace.SelectedGame.InstallationPath, "sds");
        var comparer = new ArchiveFileEqualityComparer();

        int startIndex = useCheckPoint ? LoadCheckPoint() : 0;

        for (int index = startIndex; index < files.Count; index++)
        {
            SdsFile file = files[index];
            try
            {
                TestSdsFile(
                    workspace,
                    file,
                    dataWriterScheduler,
                    dataWriterLogger,
                    comparer
                );
            }
            catch (Exception)
            {
                if (useCheckPoint)
                {
                    SaveCheckPoint(index);
                }
                
                throw;
            }
            finally
            {
                string extractPath = workspace.GetExtractPath(file.FileInfo.FullName);

                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);   
                }
            }
        }

        dataWriterScheduler.Flush();
    }

    [InlineData(
        GamesEnumerator.Mafia3,
        @"N:\games\steam\steamapps\common\Mafia III",
        @"N:\games\steam\steamapps\common\Mafia III\sds_retail\basic_anim\anim_project.sds"
    )]
    [Theory]
    public void DebugSdsFile(
        GamesEnumerator selectedGame,
        string installationPath,
        string filePath
    )
    {
        var workspace = GameWorkSpace.Create(
            selectedGame,
            installationPath,
            @"D:\mafia-sdk-workspaces"
        );
        
        var file = new SdsFile(
            new FileInfo(filePath)
        );

        NullDataWriterLogger dataWriterLogger = null!;
        DataWriterScheduler dataWriterScheduler = CreateDataWriterScheduler(
            monitor => dataWriterLogger = new NullDataWriterLogger(monitor)
        );
        var comparer = new ArchiveFileEqualityComparer();
        
        TestSdsFile(
            workspace,
            file,
            dataWriterScheduler,
            dataWriterLogger,
            comparer
        );
    }
        
    private void TestSdsFile(
        GameWorkSpace workspace,
        SdsFile file,
        DataWriterScheduler dataWriterScheduler,
        IDataWriterLogger dataWriterLogger,
        ArchiveFileEqualityComparer comparer
    )
    {
        string filePath = file.FileInfo.FullName;
        string extractPath = workspace.GetExtractPath(filePath);
        dataWriterScheduler.BasePath = extractPath;

        using FileStream fileStream = File.Open(filePath, FileMode.Open);
        using var inputStream = new MemoryStream(fileStream.ReadBytes((int)fileStream.Length));
        using var outputStream = new MemoryStream();
                
        ArchiveFile archiveFileForDeserialization = ArchiveFile.Deserialize(inputStream);

        Resource.DeserializeResources(
            dataWriterScheduler, 
            archiveFileForDeserialization, 
            GameWorkSpace.Instance().SelectedGame.Type
        );
                
        dataWriterLogger.Log();

        ArchiveFile archiveFileForSerialization = Resource.SerializeResources(extractPath);
                
        if (!comparer.Equals(archiveFileForDeserialization, archiveFileForSerialization))
        {
            throw new InvalidDataException();
        }
    }
}