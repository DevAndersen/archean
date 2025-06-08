namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerUpdatePlayerTypePacket : IServerPacket
{
    public required PlayerType PlayerType { get; init; }
}
