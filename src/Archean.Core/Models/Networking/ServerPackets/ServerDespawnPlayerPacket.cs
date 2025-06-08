namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDespawnPlayerPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.DespawnPlayer;

    public required sbyte PlayerId { get; init; }
}
