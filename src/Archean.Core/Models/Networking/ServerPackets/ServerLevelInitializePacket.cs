namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelInitializePacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.LevelInitialize;
}
