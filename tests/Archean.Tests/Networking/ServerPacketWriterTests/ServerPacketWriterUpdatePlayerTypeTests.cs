using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterUpdatePlayerTypeTests : BaseServerPacketWriterTests
{
    private static readonly ServerUpdatePlayerTypePacket ValidPacket = new()
    {
        PlayerType = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteUpdatePlayerTypePacket(ValidPacket);

        // Assert
        Assert.Equal(ServerUpdatePlayerTypePacket.PacketSize, buffer.Length);
    }
}
