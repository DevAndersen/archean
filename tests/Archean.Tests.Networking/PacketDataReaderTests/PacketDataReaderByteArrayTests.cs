using Archean.Networking.Helpers;

namespace Archean.Tests.Networking.PacketDataReaderTests;

public class PacketDataReaderByteArrayTests
{
    private const int RestBufferSize = 4;
    private const byte FillByte = 1;

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedReadBytes()
    {
        // Arrange
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + RestBufferSize];
        buffer.AsSpan()[..Constants.Networking.ByteArrayLength].Fill(FillByte);

        // Act
        byte[] readBytes = PacketDataReader.ReadByteArray(buffer, out _);

        // Assert
        byte[] expectedBytes = new byte[Constants.Networking.ByteArrayLength];
        Array.Fill(expectedBytes, FillByte);
        Assert.Equal(expectedBytes, readBytes);
    }

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedRestBufferSize()
    {
        // Arrange
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + RestBufferSize];
        buffer.AsSpan()[..Constants.Networking.ByteArrayLength].Fill(FillByte);

        // Act
        PacketDataReader.ReadByteArray(buffer, out ReadOnlySpan<byte> restBuffer);

        // Assert
        Assert.Equal(RestBufferSize, restBuffer.Length);
    }

    [Fact]
    public void ReadByteArray_BufferTooSmall_ThrowsException()
    {
        // Arrange
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength - 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketDataReader.ReadByteArray(buffer.Span, out _));
    }

    [Fact]
    public void ReadByteArray_EmptyBuffer_ThrowsException()
    {
        // Arrange
        Memory<byte> buffer = Array.Empty<byte>();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketDataReader.ReadByteArray(buffer.Span, out _));
    }
}
