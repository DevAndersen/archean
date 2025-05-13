using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketDataReaderTests;

public class PacketDataReaderByteArrayTests
{
    private const int RestBufferSize = 4;
    private const byte FillByte = 1;

    private readonly PacketDataReader _packetDataReader;

    public PacketDataReaderByteArrayTests()
    {
        _packetDataReader = new PacketDataReader();
    }

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedReadBytes()
    {
        // Setup
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + RestBufferSize];
        buffer.AsSpan()[..Constants.Networking.ByteArrayLength].Fill(FillByte);

        // Action
        byte[] readBytes = _packetDataReader.ReadByteArray(buffer, out _);

        // Assert
        byte[] expectedBytes = new byte[Constants.Networking.ByteArrayLength];
        Array.Fill(expectedBytes, FillByte);
        Assert.Equal(expectedBytes, readBytes);
    }

    [Fact]
    public void ReadByteArray_ValidBuffer_ExpectedRestBufferSize()
    {
        // Setup
        byte[] buffer = new byte[Constants.Networking.ByteArrayLength + RestBufferSize];
        buffer.AsSpan()[..Constants.Networking.ByteArrayLength].Fill(FillByte);

        // Action
        _packetDataReader.ReadByteArray(buffer, out ReadOnlyMemory<byte> restBuffer);

        // Assert
        Assert.Equal(RestBufferSize, restBuffer.Length);
    }

    [Fact]
    public void ReadByteArray_BufferTooSmall_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength - 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _packetDataReader.ReadByteArray(buffer, out _));
    }

    [Fact]
    public void ReadByteArray_EmptyBuffer_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = Array.Empty<byte>();

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _packetDataReader.ReadByteArray(buffer, out _));
    }
}
