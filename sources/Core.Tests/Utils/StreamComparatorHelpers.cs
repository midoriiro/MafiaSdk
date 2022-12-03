using System.Security.Cryptography;
using Core.Games;
using Core.IO.FileFormats.SDS;
using Core.IO.FileFormats.SDS.Resource;
using Core.IO.Streams;
using Assert = Xunit.Assert;

namespace Core.Tests.Utils;

public class StreamComparatorHelpers
{
    private static void CompareData(Stream inputStream, Stream outputStream)
    {
        inputStream.Position = 0;
        outputStream.Position = 0;
        byte[] inputData = inputStream.ReadBytes((int)inputStream.Length);
        byte[] outputData = outputStream.ReadBytes((int)outputStream.Length);
        
        // if (inputData.Length != outputData.Length)
        // {
        //     throw new InvalidDataException();
        // }

        for (var x = 0; x < inputData.Length; x++)
        {
            byte inputByte = inputData[x];
            byte outputByte = outputData[x];

            if (inputByte != outputByte)
            {
                throw new InvalidDataException();
            }
        }
        
        byte[] hashInput = SHA512.HashData(inputStream);
        byte[] hashOutput = SHA512.HashData(outputStream);
                
        Assert.Equal(hashInput, hashOutput);
    }
    
    public static void Compare(
        FileInfo fileInfo,
        Action<Stream> inputAction,
        Action<Stream> outputAction
    )
    {
        FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open);
        var inputStream = new MemoryStream(fileStream.ReadBytes((int)fileStream.Length));

        inputAction(inputStream);

        var outputStream = new MemoryStream();
        outputAction(outputStream);

        try
        {
            CompareData(inputStream, outputStream);
        }
        finally
        {
            inputStream.Dispose();
            outputStream.Dispose();
        }
    }
}