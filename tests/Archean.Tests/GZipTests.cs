using Archean.Application;

namespace Archean.Tests;

public class GZipTests
{
    public static IEnumerable<object[]> data = [];

    static GZipTests()
    {
        List<byte[]> byteSamples = [];

        byteSamples.Add([]);
        byteSamples.Add([1]);
        byteSamples.Add([1, 2, 3, 4, 5]);
        byteSamples.Add(new byte[100]);
        byteSamples.Add("The quick brown fox jumps over the lazy dog"u8.ToArray());

        data = byteSamples.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(data))]
    public void GZipHelper_CompressAndDecompress_ReturnsOriginalBytes(byte[] bytes)
    {
        // Action
        byte[] compressedBytes = GZipHelper.Compress(bytes);
        byte[] decompressedBytes = GZipHelper.Decompress(compressedBytes);

        // Assert
        Assert.Equal(bytes, decompressedBytes);
    }
}
