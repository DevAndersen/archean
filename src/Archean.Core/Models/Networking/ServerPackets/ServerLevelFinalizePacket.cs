namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelFinalizePacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // XSize
        + sizeof(short) // YSize
        + sizeof(short); // ZSize

    public ServerPacketId PacketId => ServerPacketId.LevelFinalize;

    public required short XSize { get; init; }

    public required short YSize { get; init; }

    public required short ZSize { get; init; }
}
