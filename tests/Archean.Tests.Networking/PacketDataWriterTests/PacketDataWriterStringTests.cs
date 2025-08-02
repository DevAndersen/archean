using Archean.Networking.Helpers;

namespace Archean.Tests.Networking.PacketDataWriterTests;

public class PacketDataWriterStringTests
{
    private const string EmptyString = "";
    private const string SingleWordString = "Abc";
    private const string MultiWordString = "Abc def ghi";
    private const string MaxSizedString = "0123456789012345678901234567890123456789012345678901234567890123";
    private const string OversizedString = "01234567890123456789012345678901234567890123456789012345678901234567890123456789";
    private const int IndexNotFound = -1;
    private const byte ZeroByte = 0;
    private const byte SpaceByte = (byte)' ';

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(SingleWordString)]
    [InlineData(MultiWordString)]
    [InlineData(MaxSizedString)]
    public void WriteString_ValidString_ExpectedWrittenBytes(string value)
    {
        // Arrange
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out _);

        // Assert
        Memory<byte> expectedStringBytes = Constants.Networking.ChatEncoding.GetBytes(value);
        Memory<byte> writtenBufferArea = buffer[..expectedStringBytes.Length];
        Assert.Equal(expectedStringBytes, writtenBufferArea);
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(SingleWordString)]
    [InlineData(MultiWordString)]
    [InlineData(MaxSizedString)]
    public void WriteString_ValidString_ExpectedRestBufferLength(string value)
    {
        // Arrange
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out Span<byte> restBuffer);

        // Assert
        Assert.Equal(additionalBufferSize, restBuffer.Length);
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(SingleWordString)]
    [InlineData(MultiWordString)]
    [InlineData(MaxSizedString)]
    public void WriteString_ValidString_ZeroFilledRestBuffer(string value)
    {
        // Arrange
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out Span<byte> restBuffer);

        // Assert
        int firstNonZeroByteInRestBuffer = restBuffer.IndexOfAnyExcept((byte)0);
        Assert.Equal(IndexNotFound, firstNonZeroByteInRestBuffer);
    }

    [Theory]
    [InlineData(EmptyString)]
    [InlineData(SingleWordString)]
    [InlineData(MultiWordString)]
    [InlineData(MaxSizedString)]
    public void WriteString_ValidString_SpaceFilledUnwrittenBuffer(string value)
    {
        // Arrange
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out _);

        // Assert
        Memory<byte> expectedStringBytes = Constants.Networking.ChatEncoding.GetBytes(value);
        Memory<byte> unwrittenBufferArea = buffer[expectedStringBytes.Length..Constants.Networking.StringLength];
        int firstNonSpaceByteInUnwrittenBuffer = unwrittenBufferArea.Span.IndexOfAnyExcept(SpaceByte);
        Assert.Equal(IndexNotFound, firstNonSpaceByteInUnwrittenBuffer);
    }

    [Fact]
    public void WriteString_StringTooLong_ExpectedTruncatedWrittenBytes()
    {
        // Arrange
        string value = OversizedString;
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out _);

        // Assert
        Memory<byte> expectedStringBytes = Constants.Networking.ChatEncoding.GetBytes(value).AsMemory()[..Constants.Networking.StringLength];
        Memory<byte> writtenBufferArea = buffer[..expectedStringBytes.Length];
        Assert.Equal(expectedStringBytes, writtenBufferArea);
    }

    [Fact]
    public void WriteString_StringTooLong_ZeroFilledRestBuffer()
    {
        // Arrange
        string value = OversizedString;
        int additionalBufferSize = 4;
        Memory<byte> buffer = new byte[Constants.Networking.StringLength + additionalBufferSize];

        // Act
        PacketDataWriter.WriteString(value, buffer.Span, out Span<byte> restBuffer);

        // Assert
        int firstNonNullByteInRestBuffer = restBuffer.IndexOfAnyExcept(ZeroByte);
        Assert.Equal(IndexNotFound, firstNonNullByteInRestBuffer);
    }

    [Fact]
    public void WriteString_BufferTooSmall_ThrowsException()
    {
        // Arrange
        Memory<byte> undersizedBuffer = new byte[Constants.Networking.StringLength - 1];
        string value = SingleWordString;

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PacketDataWriter.WriteString(value, undersizedBuffer.Span, out _));
    }
}
