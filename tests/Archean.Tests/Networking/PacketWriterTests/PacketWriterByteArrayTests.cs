using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketWriterTests;

public class PacketWriterByteArrayTests
{
    private const int indexNotFound = -1;
    private const byte zeroByte = 0;

    public static readonly byte[] shortByteArray;
    public static readonly byte[] emptyByteArray;
    public static readonly byte[] filledByteArray;

    public static TheoryData<byte[]> ValidInputData => new TheoryData<byte[]>
    {
        emptyByteArray,
        shortByteArray,
        filledByteArray
    };

    static PacketWriterByteArrayTests()
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
        PacketWriter.WriteByteArray(data, buffer, out _);

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
        PacketWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

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
        PacketWriter.WriteByteArray(data, buffer, out Memory<byte> restBuffer);

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
        PacketWriter.WriteByteArray(data, buffer, out _);

        // Assert
        Memory<byte> unwrittenBufferArea = buffer.Slice(data.Length, Constants.Networking.ByteArrayLength - data.Length);
        int firstNonZeroByteInUnwrittenBuffer = unwrittenBufferArea.Span.IndexOfAnyExcept(zeroByte);
        Assert.Equal(indexNotFound, firstNonZeroByteInUnwrittenBuffer);
    }

    [Fact]
    public void WriteByteArray_DataTooLong_ThrowsException()
    {
        // Setup
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength + 8];
        byte[] value = new byte[Constants.Networking.ByteArrayLength + 2];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketWriter.WriteByteArray(value, undersizedBuffer, out _));
    }

    [Fact]
    public void WriteByteArray_BufferTooShort_ThrowsException()
    {
        // Setup
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.ByteArrayLength - 1];
        byte[] value = shortByteArray;

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketWriter.WriteByteArray(value, undersizedBuffer, out _));
    }
}
