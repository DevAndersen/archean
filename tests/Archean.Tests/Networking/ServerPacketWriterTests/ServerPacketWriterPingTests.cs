using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterPingTests : BaseServerPacketWriterTests
{
    private static readonly ServerPingPacket validPacket = new();

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WritePingPacket(validPacket);

        // Assert
        Assert.Equal(ServerPingPacket.PacketSize, buffer.Length);
    }
}
