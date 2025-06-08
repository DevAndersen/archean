namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDisconnectPlayerPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.DisconnectPlayer;

    public required string Message { get; init; }
}
