using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterRelativeOrientationTests : BaseServerPacketWriterTests
{
    private static readonly ServerRelativeOrientationPacket ValidPacket = new()
    {
        PlayerId = default,
        Yaw = default,
        Pitch = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteRelativeOrientationPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerRelativeOrientationPacket.PacketSize, buffer.Length);
    }
}
