using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterSpawnPlayerTests : BaseServerPacketWriterTests
{
    private static readonly ServerSpawnPlayerPacket ValidPacket = new()
    {
        PlayerId = default,
        PlayerName = string.Empty,
        X = default,
        Y = default,
        Z = default,
        Yaw = default,
        Pitch = default,
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteSpawnPlayerPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerSpawnPlayerPacket.PacketSize, buffer.Length);
    }
}
