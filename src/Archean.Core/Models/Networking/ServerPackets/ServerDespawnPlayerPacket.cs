namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerDespawnPlayerPacket : IServerPacket
{
    public required sbyte PlayerId { get; init; }
}
