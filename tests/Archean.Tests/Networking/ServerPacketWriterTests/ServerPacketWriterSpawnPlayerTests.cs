using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterSpawnPlayerTests : BaseServerPacketWriterTests
{
    private static readonly ServerSpawnPlayerPacket validPacket = new()
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
        ReadOnlyMemory<byte> buffer = writer.WriteSpawnPlayerPacket(validPacket);

        // Assert
        Assert.Equal(ServerSpawnPlayerPacket.PacketSize, buffer.Length);
    }
}
