using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterLevelDataChunkTests : BaseServerPacketWriterTests
{
    private static readonly ServerLevelDataChunkPacket validPacket = new()
    {
        ChunkLength = default,
        ChunkData = new byte[Constants.Networking.ByteArrayLength],
        PercentageComplete = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteLevelDataChunkPacket(validPacket);

        // Assert
        Assert.Equal(ServerLevelDataChunkPacket.PacketSize, buffer.Length);
    }
}
