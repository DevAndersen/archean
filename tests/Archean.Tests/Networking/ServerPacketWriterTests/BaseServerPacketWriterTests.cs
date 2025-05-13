using Archean.Application.Services.Networking;

namespace Archean.Tests.Networking.ServerPacketWriterTests;

public abstract class BaseServerPacketWriterTests
{
    protected readonly ServerPacketWriter _writer;

    public BaseServerPacketWriterTests()
    {
        PacketDataWriter dataWriter = new PacketDataWriter();
        _writer = new ServerPacketWriter(dataWriter);
    }
}
