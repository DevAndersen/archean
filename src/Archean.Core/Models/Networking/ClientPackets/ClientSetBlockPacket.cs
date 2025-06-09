namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientSetBlockPacket : IClientPacket
{
    public required short X { get; init; }

    public required short Y { get; init; }

    public required short Z { get; init; }

    public required BlockChangeMode Mode { get; init; }

    public required Block BlockType { get; init; }
}
