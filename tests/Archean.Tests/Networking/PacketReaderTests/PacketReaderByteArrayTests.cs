using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketReaderTests;

public class PacketReaderByteArrayTests
{
    private const int restBufferSize = 4;
    private const byte fillByte = 1;

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedReadBytes()
    {
        // Setup
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + restBufferSize];
        buffer.AsSpan().Slice(0, Constants.Networking.ByteArrayLength).Fill(fillByte);

        // Action
        byte[] readBytes = PacketReader.ReadByteArray(buffer, out _);

        // Assert
        byte[] expectedBytes = new byte[Constants.Networking.ByteArrayLength];
        Array.Fill(expectedBytes, fillByte);
        Assert.Equal(expectedBytes, readBytes);
    }

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedRestBufferSize()
    {
        // Setup
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + restBufferSize];
        buffer.AsSpan().Slice(0, Constants.Networking.ByteArrayLength).Fill(fillByte);

        // Action
        PacketReader.ReadByteArray(buffer, out ReadOnlyMemory<byte> restBuffer);

        // Assert
        Assert.Equal(restBufferSize, restBuffer.Length);
    }

    [Fact]
    public void ReadByteArray_BufferTooSmall_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength - 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketReader.ReadByteArray(buffer, out _));
    }

    [Fact]
    public void ReadByteArray_EmptyBuffer_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = Array.Empty<byte>();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketReader.ReadByteArray(buffer, out _));
    }
}
