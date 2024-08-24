namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerMessagePacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + Constants.Networking.StringLength; // Message

    public ServerPacketId PacketId => ServerPacketId.Message;

    public required sbyte PlayerId { get; init; }

    public required string Message { get; init; }
}
