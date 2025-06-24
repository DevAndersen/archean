using Archean.Core.Helpers;

namespace Archean.Tests.Core;

public class GZipTests
{
    public static IEnumerable<TheoryDataRow<byte[]>> Data =>
    [
        new ([]),
        new ([1]),
        new ([1, 2, 3, 4, 5]),
        new (new byte[100]),
        new ("The quick brown fox jumps over the lazy dog"u8.ToArray())
    ];

    [Theory]
    [MemberData(nameof(Data))]
    public void GZipHelper_CompressAndDecompress_ReturnsOriginalBytes(byte[] bytes)
    {
        // Act
        byte[] compressedBytes = GZipHelper.Compress(bytes);
        byte[] decompressedBytes = GZipHelper.Decompress(compressedBytes);

        // Assert
        Assert.Equal(bytes, decompressedBytes);
    }
}
