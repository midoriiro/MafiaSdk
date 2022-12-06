using System.Security.Cryptography;
using Core.Games;
using Core.IO.FileFormats.SDS;
using Core.IO.FileFormats.SDS.Resource;
using Core.IO.Streams;
using Assert = Xunit.Assert;

namespace Core.Tests.Utils;

public class StreamComparatorHelpers
{
    public static bool CompareData(Stream inputStream, Stream outputStream)
    {
        inputStream.Position = 0;
        outputStream.Position = 0;
        byte[] inputData = inputStream.ReadBytes((int)inputStream.Length);
        byte[] outputData = outputStream.ReadBytes((int)outputStream.Length);
        
        if (inputData.Length != outputData.Length)
        {
            return false;
        }

        for (var x = 0; x < inputData.Length; x++)
        {
            byte inputByte = inputData[x];
            byte outputByte = outputData[x];

            if (inputByte != outputByte)
            {
                return false;
            }
        }
        
        byte[] hashInput = SHA512.HashData(inputStream);
        byte[] hashOutput = SHA512.HashData(outputStream);

        if (hashInput != hashOutput)
        {
            return false;
        }

        return true;
    }
}