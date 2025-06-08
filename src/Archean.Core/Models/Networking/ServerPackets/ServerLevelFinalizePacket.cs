namespace Archean.Core.Models.Networking.ServerPackets;

public class ServerLevelFinalizePacket : IServerPacket
{
    public required short XSize { get; init; }

    public required short YSize { get; init; }

    public required short ZSize { get; init; }
}
