using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketReaderTests;

public class PacketReaderStringTests
{
    private static readonly string shortString = "Test string";
    private static readonly string filledString = new string('A', 64);

    private static readonly byte[] emptyStringBuffer;
    private static readonly byte[] shortStringBuffer;
    private static readonly byte[] filledStringBuffer;

    private static readonly int additionalBufferSize = 4;

    public static TheoryData<byte[], string> DataSets => new()
    {
        { emptyStringBuffer, string.Empty },
        { shortStringBuffer, shortString },
        { filledStringBuffer, filledString }
    };

    private readonly PacketReader packetReader;

    public PacketReaderStringTests()
    {
        packetReader = new PacketReader();
    }

    static PacketReaderStringTests()
    {
        int bufferSize = Constants.Networking.StringLength + additionalBufferSize;
        emptyStringBuffer = new byte[bufferSize];
        shortStringBuffer = new byte[bufferSize];
        filledStringBuffer = new byte[bufferSize];

        Encoding.ASCII.GetBytes(shortString, shortStringBuffer);
        Encoding.ASCII.GetBytes(filledString, filledStringBuffer);
    }

    [Theory]
    [MemberData(nameof(DataSets))]
    public void ReadString_ValidBuffer_ExpectedReadString(byte[] data, string originalString)
    {
        // Setup
        Memory<byte> buffer = data;

        // Action
        string readString = packetReader.ReadString(buffer, out _);

        // Assert
        Assert.Equal(originalString, readString);
    }

    [Theory]
    [MemberData(nameof(DataSets))]
    public void ReadString_ValidBuffer_ExpectedRestBufferLength(byte[] data, string _)
    {
        // Setup
        Memory<byte> buffer = data;

        // Action
        packetReader.ReadString(buffer, out ReadOnlyMemory<byte> restBuffer);

        // Assert
        Assert.Equal(additionalBufferSize, restBuffer.Length);
    }

    [Fact]
    public void ReadString_BufferTooSmall_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.StringLength - 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => packetReader.ReadString(buffer, out ReadOnlyMemory<byte> restBuffer));
    }
}
