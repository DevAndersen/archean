namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerPingPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.Ping;
}
