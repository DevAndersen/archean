using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketDataWriterTests;

public class PacketDataWriterStringTests
{
    private const string emptyString = "";
    private const string singleWordString = "Abc";
    private const string multiWordString = "Abc def ghi";
    private const string maxSizedString = "0123456789012345678901234567890123456789012345678901234567890123";
    private const string oversizedString = "01234567890123456789012345678901234567890123456789012345678901234567890123456789";
    private const int indexNotFound = -1;
    private const byte zeroByte = 0;
    private const byte spaceByte = (byte)' ';

    private readonly PacketDataWriter packetDataWriter;

    public PacketDataWriterStringTests()
    {
        packetDataWriter = new PacketDataWriter();
    }

    [Theory]
    [InlineData(emptyString)]
    [InlineData(singleWordString)]
    [InlineData(multiWordString)]
    [InlineData(maxSizedString)]
    public void WriteString_ValidString_ExpectedWrittenBytes(string value)
    {
        // Setup
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out _);

        // Assert
        Memory<byte> expectedStringBytes = Encoding.UTF8.GetBytes(value);
        Memory<byte> writtenBufferArea = buffer[..expectedStringBytes.Length];
        Assert.Equal(expectedStringBytes, writtenBufferArea);
    }

    [Theory]
    [InlineData(emptyString)]
    [InlineData(singleWordString)]
    [InlineData(multiWordString)]
    [InlineData(maxSizedString)]
    public void WriteString_ValidString_ExpectedRestBufferLength(string value)
    {
        // Setup
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out Memory<byte> restBuffer);

        // Assert
        Assert.Equal(additionalBufferSize, restBuffer.Length);
    }

    [Theory]
    [InlineData(emptyString)]
    [InlineData(singleWordString)]
    [InlineData(multiWordString)]
    [InlineData(maxSizedString)]
    public void WriteString_ValidString_ZeroFilledRestBuffer(string value)
    {
        // Setup
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out Memory<byte> restBuffer);

        // Assert
        int firstNonZeroByteInRestBuffer = restBuffer.Span.IndexOfAnyExcept((byte)0);
        Assert.Equal(indexNotFound, firstNonZeroByteInRestBuffer);
    }

    [Theory]
    [InlineData(emptyString)]
    [InlineData(singleWordString)]
    [InlineData(multiWordString)]
    [InlineData(maxSizedString)]
    public void WriteString_ValidString_SpaceFilledUnwrittenBuffer(string value)
    {
        // Setup
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out _);

        // Assert
        Memory<byte> expectedStringBytes = Encoding.UTF8.GetBytes(value);
        Memory<byte> unwrittenBufferArea = buffer[expectedStringBytes.Length..Constants.Networking.StringLength];
        int firstNonSpaceByteInUnwrittenBuffer = unwrittenBufferArea.Span.IndexOfAnyExcept(spaceByte);
        Assert.Equal(indexNotFound, firstNonSpaceByteInUnwrittenBuffer);
    }

    [Fact]
    public void WriteString_StringTooLong_ExpectedTruncatedWrittenBytes()
    {
        // Setup
        string value = oversizedString;
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out _);

        // Assert
        Memory<byte> expectedStringBytes = Encoding.UTF8.GetBytes(value).AsMemory()[..Constants.Networking.StringLength];
        Memory<byte> writtenBufferArea = buffer[..expectedStringBytes.Length];
        Assert.Equal(expectedStringBytes, writtenBufferArea);
    }

    [Fact]
    public void WriteString_StringTooLong_ZeroFilledRestBuffer()
    {
        // Setup
        string value = oversizedString;
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Action
        packetDataWriter.WriteString(value, buffer, out Memory<byte> restBuffer);

        // Assert
        int firstNonNullByteInRestBuffer = restBuffer.Span.IndexOfAnyExcept(zeroByte);
        Assert.Equal(indexNotFound, firstNonNullByteInRestBuffer);
    }

    [Fact]
    public void WriteString_BufferTooSmall_ThrowsException()
    {
        // Setup
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.StringLength - 1];
        string value = singleWordString;

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => packetDataWriter.WriteString(value, undersizedBuffer, out _));
    }
}
