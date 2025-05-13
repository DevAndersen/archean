using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterRelativePositionTests : BaseServerPacketWriterTests
{
    private static readonly ServerRelativePositionPacket ValidPacket = new()
    {
        PlayerId = default,
        X = default,
        Y = default,
        Z = default
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteRelativePositionPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerRelativePositionPacket.PacketSize, buffer.Length);
    }
}
