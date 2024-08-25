using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public class ServerPacketWriterDisconnectPlayerTests : BaseServerPacketWriterTests
{
    private static readonly ServerDisconnectPlayerPacket validPacket = new()
    {
        Message = string.Empty
    };

    [Fact]
    public void WritePacket_ValidPacket_ExpectedBufferSize()
    {
        // Action
        ReadOnlyMemory<byte> buffer = writer.WriteDisconnectPlayerPacket(validPacket);

        // Assert
        Assert.Equal(ServerDisconnectPlayerPacket.PacketSize, buffer.Length);
    }
}
