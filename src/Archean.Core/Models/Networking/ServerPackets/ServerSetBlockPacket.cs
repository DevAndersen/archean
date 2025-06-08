namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerSetBlockPacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.SetBlock;

    public required short X { get; init; }

    public required short Y { get; init; }

    public required short Z { get; init; }

    public required Block BlockType { get; init; }
}
