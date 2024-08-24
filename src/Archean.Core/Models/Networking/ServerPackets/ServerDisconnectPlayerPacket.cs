namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDisconnectPlayerPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + Constants.Networking.StringLength; // Message

    public ServerPacketId PacketId => ServerPacketId.DisconnectPlayer;

    public required string Message { get; init; }
}
