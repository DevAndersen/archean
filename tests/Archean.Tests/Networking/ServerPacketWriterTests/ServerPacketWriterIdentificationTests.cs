using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterIdentificationTests : BaseServerPacketWriterTests
{
    private static readonly ServerIdentificationPacket validPacket = new()
    {
        ProtocolVersion = default,
        ServerName = string.Empty,
        ServerMotd = string.Empty,
        PlayerType = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteIdentificationPacket(validPacket);

        // Assert
        Assert.Equal(ServerIdentificationPacket.PacketSize, buffer.Length);
    }
}
