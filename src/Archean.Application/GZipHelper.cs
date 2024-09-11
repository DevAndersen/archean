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
}
