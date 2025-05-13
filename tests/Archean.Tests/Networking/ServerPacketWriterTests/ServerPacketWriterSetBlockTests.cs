using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterSetBlockTests : BaseServerPacketWriterTests
{
    private static readonly ServerSetBlockPacket ValidPacket = new()
    {
        X = default,
        Y = default,
        Z = default,
        BlockType = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteSetBlockPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerSetBlockPacket.PacketSize, buffer.Length);
    }
}
