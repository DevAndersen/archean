namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelDataChunkPacket : IServerPacket
{
    public required short ChunkLength { get; init; }

    public required byte[] ChunkData { get; init; }

    public required byte PercentageComplete { get; init; }
}
