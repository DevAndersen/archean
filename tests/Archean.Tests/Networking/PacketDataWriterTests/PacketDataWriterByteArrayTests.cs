using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketDataWriterTests;

public class PacketDataWriterByteArrayTests
{
    private const int IndexNotFound = -1;
    private const byte ZeroByte = 0;

    private static readonly byte[] ShortByteArray;
    private static readonly byte[] EmptyByteArray;
    private static readonly byte[] FilledByteArray;

    private readonly PacketDataWriter _packetDataWriter;

    public PacketDataWriterByteArrayTests()
    {
        _packetDataWriter = new PacketDataWriter();
    }

    public static TheoryData<byte[]> ValidInputData => new TheoryData<byte[]>
    {
        EmptyByteArray,
        ShortByteArray,
        FilledByteArray
    };

    static PacketDataWriterByteArrayTests()
    {
        EmptyByteArray = [];
        ShortByteArray = [1, 2, 3, 4];

        FilledByteArray = new byte[Constants.Networking.ByteArrayLength];
        Array.Fill<byte>(FilledByteArray, 127);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ExpectedWrittenBytes(byte[] data)
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength];

        // Action
        _packetDataWriter.WriteByteArray(data, buffer, out _);

        // Assert
        Assert.Equal(data, buffer[..data.Length]);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ExpectedRestBufferLength(byte[] data)
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];

        // Action
        _packetDataWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

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
        _packetDataWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

        // Assert
        int firstNonZeroByteInRestBuffer = restBuffer.Span.IndexOfAnyExcept(ZeroByte);
        Assert.Equal(IndexNotFound, firstNonZeroByteInRestBuffer);
    }

    [Theory]
    [MemberData(nameof(ValidInputData))]
    public void WriteByteArray_ValidData_ZeroFilledUnwrittenBuffer(byte[] data)
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> buffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];

        // Action
        _packetDataWriter.WriteByteArray(data, buffer, out _);

        // Assert
        Memory<byte> unwrittenBufferArea = buffer[data.Length..Constants.Networking.ByteArrayLength];
        int firstNonZeroByteInUnwrittenBuffer = unwrittenBufferArea.Span.IndexOfAnyExcept(ZeroByte);
        Assert.Equal(IndexNotFound, firstNonZeroByteInUnwrittenBuffer);
    }

    [Fact]
    public void WriteByteArray_DataTooLong_ThrowsException()
    {
        // Setup
        int extraBytes = 4;
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength + extraBytes];
        byte[] value = new byte[Constants.Networking.ByteArrayLength + 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _packetDataWriter.WriteByteArray(value, undersizedBuffer, out _));
    }

    [Fact]
    public void WriteByteArray_BufferTooShort_ThrowsException()
    {
        // Setup
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength - 1];
        byte[] value = ShortByteArray;

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _packetDataWriter.WriteByteArray(value, undersizedBuffer, out _));
    }
}
