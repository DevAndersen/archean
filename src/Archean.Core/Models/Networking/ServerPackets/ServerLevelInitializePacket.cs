namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelInitializePacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId); // Packet ID

    public ServerPacketId PacketId => ServerPacketId.LevelInitialize;
}
