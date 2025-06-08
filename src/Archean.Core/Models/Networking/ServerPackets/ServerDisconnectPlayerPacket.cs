namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDisconnectPlayerPacket : IServerPacket
{
    public required string Message { get; init; }
}
