using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterMessageTests : BaseServerPacketWriterTests
{
    private static readonly ServerMessagePacket ValidPacket = new()
    {
        PlayerId = default,
        Message = string.Empty
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteMessagePacket(ValidPacket);

        // Assert
        Assert.Equal(ServerMessagePacket.PacketSize, buffer.Length);
    }
}
