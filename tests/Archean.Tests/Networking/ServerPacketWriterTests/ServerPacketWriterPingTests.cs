using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterPingTests : BaseServerPacketWriterTests
{
    private static readonly ServerPingPacket ValidPacket = new();

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WritePingPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerPingPacket.PacketSize, buffer.Length);
    }
}
