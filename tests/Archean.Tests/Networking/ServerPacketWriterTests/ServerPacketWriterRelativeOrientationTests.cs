using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterRelativeOrientationTests : BaseServerPacketWriterTests
{
    private static readonly ServerRelativeOrientationPacket validPacket = new()
    {
        PlayerId = default,
        Yaw = default,
        Pitch = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteRelativeOrientationPacket(validPacket);

        // Assert
        Assert.Equal(ServerRelativeOrientationPacket.PacketSize, buffer.Length);
    }
}
