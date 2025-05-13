using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterDisconnectPlayerTests : BaseServerPacketWriterTests
{
    private static readonly ServerDisconnectPlayerPacket ValidPacket = new()
    {
        Message = string.Empty
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = _writer.WriteDisconnectPlayerPacket(ValidPacket);

        // Assert
        Assert.Equal(ServerDisconnectPlayerPacket.PacketSize, buffer.Length);
    }
}
