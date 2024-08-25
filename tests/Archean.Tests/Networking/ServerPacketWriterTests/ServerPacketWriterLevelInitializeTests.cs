using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterLevelInitializeTests : BaseServerPacketWriterTests
{
    private static readonly ServerLevelInitializePacket validPacket = new();

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteLevelInitializePacket(validPacket);

        // Assert
        Assert.Equal(ServerLevelInitializePacket.PacketSize, buffer.Length);
    }
}
