using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterDespawnPlayerTests : BaseServerPacketWriterTests
{
    private static readonly ServerDespawnPlayerPacket ValidPacket = new()
    {
        PlayerId = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteDespawnPlayerPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerDespawnPlayerPacket.PacketSize, buffer.Length);
    }
}
