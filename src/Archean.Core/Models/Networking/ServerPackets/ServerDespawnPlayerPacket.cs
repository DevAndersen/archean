namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDespawnPlayerPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte); // Player ID

    public ServerPacketId PacketId => ServerPacketId.DespawnPlayer;

    public required sbyte PlayerId { get; init; }
}
