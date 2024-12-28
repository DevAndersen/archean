using System.IO.Compression;

namespace Archean.Application;

public static class GZipHelper
{
    public static byte[] Compress(ReadOnlySpan<byte> bytes)
    {
        using MemoryStream output = new MemoryStream();
        using (GZipStream gZipStream = new GZipStream(output, CompressionLevel.Optimal))
        {
            gZipStream.Write(bytes);
        }

        return output.ToArray();
    }

    public static byte[] Decompress(byte[] bytes)
    {
        using MemoryStream input = new MemoryStream(bytes);
        using MemoryStream output = new MemoryStream();
        using GZipStream gZipStream = new GZipStream(input, CompressionMode.Decompress);
        gZipStream.CopyTo(output);
        return output.ToArray();
    }
}
