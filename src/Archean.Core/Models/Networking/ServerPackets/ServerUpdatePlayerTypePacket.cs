namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerUpdatePlayerTypePacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.UpdatePlayerType;

    public required PlayerType PlayerType { get; init; }
}
