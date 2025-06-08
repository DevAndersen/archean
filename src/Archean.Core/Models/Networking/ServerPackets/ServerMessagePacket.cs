namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerMessagePacket : IServerPacket
{
    public ServerPacketId PacketId => ServerPacketId.Message;

    public required sbyte PlayerId { get; init; }

    public required string Message { get; init; }
}
