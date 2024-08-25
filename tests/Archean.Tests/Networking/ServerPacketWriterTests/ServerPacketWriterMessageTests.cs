using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterMessageTests : BaseServerPacketWriterTests
{
    private static readonly ServerMessagePacket validPacket = new()
    {
        PlayerId = default,
        Message = string.Empty
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteMessagePacket(validPacket);

        // Assert
        Assert.Equal(ServerMessagePacket.PacketSize, buffer.Length);
    }
}
