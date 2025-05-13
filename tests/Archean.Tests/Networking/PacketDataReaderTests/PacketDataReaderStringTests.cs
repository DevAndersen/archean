using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.PacketDataReaderTests;

public class PacketDataReaderStringTests
{
    private static readonly string ShortString = "Test string";
    private static readonly string FilledString = new string('A', 64);

    private static readonly byte[] EmptyStringBuffer;
    private static readonly byte[] ShortStringBuffer;
    private static readonly byte[] FilledStringBuffer;

    private static readonly int AdditionalBufferSize = 4;

    public static TheoryData<byte[], string> DataSets => new()
    {
        { EmptyStringBuffer, string.Empty },
        { ShortStringBuffer, ShortString },
        { FilledStringBuffer, FilledString }
    };

    private readonly PacketDataReader _packetDataReader;

    public PacketDataReaderStringTests()
    {
        _packetDataReader = new PacketDataReader();
    }

    static PacketDataReaderStringTests()
    {
        int bufferSize = Constants.Networking.StringLength + AdditionalBufferSize;
        EmptyStringBuffer = new byte[bufferSize];
        ShortStringBuffer = new byte[bufferSize];
        FilledStringBuffer = new byte[bufferSize];

        Encoding.UTF8.GetBytes(ShortString, ShortStringBuffer);
        Encoding.UTF8.GetBytes(FilledString, FilledStringBuffer);
    }

    [Theory]
    [MemberData(nameof(DataSets))]
    public void ReadString_ValidBuffer_ExpectedReadString(byte[] data, string originalString)
    {
        // Setup
        Memory<byte> buffer = data;

        // Action
        string readString = _packetDataReader.ReadString(buffer, out _);

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
        _packetDataReader.ReadString(buffer, out ReadOnlyMemory<byte> restBuffer);

        // Assert
        Assert.Equal(AdditionalBufferSize, restBuffer.Length);
    }

    [Fact]
    public void ReadString_BufferTooSmall_ThrowsException()
    {
        // Setup
        Memory<byte> buffer = new byte[Constants.Networking.StringLength - 1];

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _packetDataReader.ReadString(buffer, out ReadOnlyMemory<byte> restBuffer));
    }
}
