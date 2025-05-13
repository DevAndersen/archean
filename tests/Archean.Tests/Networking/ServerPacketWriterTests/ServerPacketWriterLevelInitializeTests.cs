using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterLevelInitializeTests : BaseServerPacketWriterTests
{
    private static readonly ServerLevelInitializePacket ValidPacket = new();

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteLevelInitializePacket(ValidPacket);

        // Assert
        Assert.Equal(ServerLevelInitializePacket.PacketSize, buffer.Length);
    }
}
