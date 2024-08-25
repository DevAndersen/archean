using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterAbsolutePositionAndOrientationTests : BaseServerPacketWriterTests
{
    private static readonly ServerAbsolutePositionAndOrientationPacket validPacket = new()
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
        ReadOnlyMemory<byte> buffer = writer.WriteAbsolutePositionAndOrientationPacket(validPacket);

        // Assert
        Assert.Equal(ServerAbsolutePositionAndOrientationPacket.PacketSize, buffer.Length);
    }
}
