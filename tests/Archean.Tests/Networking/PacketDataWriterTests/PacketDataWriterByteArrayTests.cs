using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketDataWriterTests;

public class PacketDataWriterByteArrayTests
{
    private const int indexNotFound = -1;
    private const byte zeroByte = 0;

    private static readonly byte[] shortByteArray;
    private static readonly byte[] emptyByteArray;
    private static readonly byte[] filledByteArray;

    private readonly PacketDataWriter packetDataWriter;

    public PacketDataWriterByteArrayTests()
    {
        packetDataWriter = new PacketDataWriter();
    }

    public static TheoryData<byte[]> ValidInputData => new TheoryData<byte[]>
    {
        emptyByteArray,
        shortByteArray,
        filledByteArray
    };

    static PacketDataWriterByteArrayTests()
    {
        emptyByteArray = [];
        shortByteArray = [1, 2, 3, 4];

        filledByteArray = new byte[Constants.Networking.ByteArrayLength];
        Array.Fill<byte>(filledByteArray, 127);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ExpectedWrittenBytes(byte[] data)
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength];

        // Action
        packetDataWriter.WriteByteArray(data, buffer, out _);

        // Assert
        Assert.Equal(data, buffer.Slice(0, data.Length));
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ExpectedRestBufferLength(byte[] data)
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];

        // Action
        packetDataWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

        // Assert
        Assert.Equal(extraBytes, restBuffer.Length);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ZeroFilledRestBuffer(byte[] data)
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];

        // Action
        packetDataWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

        // Assert
        int firstNonZeroByteInRestBuffer = restBuffer.Span.IndexOfAnyExcept(zeroByte);
        Assert.Equal(indexNotFound, firstNonZeroByteInRestBuffer);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ZeroFilledUnwrittenBuffer(byte[] data)
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];

        // Action
        packetDataWriter.WriteByteArray(data, buffer, out _);

        // Assert
        Memory<byte> unwrittenBufferArea = buffer.Slice(data.Length, Constants.Networking.ByteArrayLength - data.Length);
        int firstNonZeroByteInUnwrittenBuffer = unwrittenBufferArea.Span.IndexOfAnyExcept(zeroByte);
        Assert.Equal(indexNotFound, firstNonZeroByteInUnwrittenBuffer);
    }

    [Fact]
    public void WriteByteArray_DataTooLong_ThrowsException()
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];
        byte[] value = new byte[Constants.Networking.ByteArrayLength + 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => packetDataWriter.WriteByteArray(value, undersizedBuffer, out _));
    }

    [Fact]
    public void WriteByteArray_BufferTooShort_ThrowsException()
    {
        // Setup
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength - 1];
        byte[] value = shortByteArray;

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => packetDataWriter.WriteByteArray(value, undersizedBuffer, out _));
    }
}
