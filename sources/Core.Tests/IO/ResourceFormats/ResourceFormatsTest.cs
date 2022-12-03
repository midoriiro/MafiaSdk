using System.Security.Cryptography;
using Core.Games;
using Core.IO.Files;
using Core.IO.Files.Scanner;
using Core.IO.ResourceFormats.XBin;
using Core.IO.Streams;
using Assert = Xunit.Assert;

namespace Core.Tests;

public class ResourceFormatsTest
{
    private static void CompareData(byte[] input, byte[] output)
    {
        if (input.Length != output.Length)
        {
            throw new InvalidDataException();
        }

        for (var x = 0; x < input.Length; x++)
        {
            byte inputByte = input[x];
            byte outputByte = output[x];

            if (inputByte != outputByte)
            {
                throw new InvalidDataException();
            }
        }
        
        byte[] hashInput = SHA512.HashData(input);
        byte[] hashOutput = SHA512.HashData(output);
                
        Assert.Equal(hashInput, hashOutput);
    }

    [Fact]
    public void ReadXbin()
    {
        var workspace = GameWorkSpace.Create(
            GamesEnumerator.Mafia3,
            @"N:\games\steam\steamapps\common\Mafia III",
            @"D:\mafia-sdk-workspaces"
        );

        var files = FileScanner.ScanByExtension<XbinFile>(
            workspace.SelectedGame.InstallationPath, 
            "xbin"
        );

        foreach (XbinFile file in files)
        {
            try
            {
                FileStream fileStream = System.IO.File.Open(file.FileInfo.FullName, FileMode.Open);
                MemoryStream streamInput = new MemoryStream(fileStream.ReadBytes((int)fileStream.Length));
                byte[] input = streamInput.ReadBytes((int)streamInput.Length);

                streamInput.Position = 0;
                using var reader = new BinaryReader(streamInput);
                XBin xbin = XBin.ReadFromFile(reader);

                var streamOutput = new MemoryStream();
                xbin.WriteToStream(streamOutput, true);
                streamOutput.Position = 0;
                byte[] output = streamOutput.ReadBytes((int)streamOutput.Length);

                CompareData(input, output);
            }
            catch (NotImplementedException e)
            {
                // TODO add logger to xunit
                continue;
            }
        }
    }
}