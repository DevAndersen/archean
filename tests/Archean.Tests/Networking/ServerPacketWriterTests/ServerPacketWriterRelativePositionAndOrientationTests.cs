using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterRelativePositionAndOrientationTests : BaseServerPacketWriterTests
{

    private static readonly ServerRelativePositionAndOrientationPacket ValidPacket = new()
    {
        PlayerId = default,
        X = default,
        Y = default,
        Z = default,
        Yaw = default,
        Pitch = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteRelativePositionAndOrientationPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerRelativePositionAndOrientationPacket.PacketSize, buffer.Length);
    }
}
