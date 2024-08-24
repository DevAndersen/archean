namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelDataChunkPacket : IServerPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // ChunkLength
        + Constants.Networking.ByteArrayLength // ChunkData
        + sizeof(byte); // PercentageComplete

    public ServerPacketId PacketId => ServerPacketId.LevelDataChunk;

    public required short ChunkLength { get; init; }

    public required byte[] ChunkData { get; init; }

    public required byte PercentageComplete { get; init; }
}
