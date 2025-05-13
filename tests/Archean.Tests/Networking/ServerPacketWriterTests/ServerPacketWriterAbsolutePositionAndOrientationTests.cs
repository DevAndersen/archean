using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterAbsolutePositionAndOrientationTests : BaseServerPacketWriterTests
{
    private static readonly ServerAbsolutePositionAndOrientationPacket ValidPacket = new()
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
        ReadOnlyMemory<byte> buffer = _writer.WriteAbsolutePositionAndOrientationPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerAbsolutePositionAndOrientationPacket.PacketSize, buffer.Length);
    }
}
