namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientMessagePacket : IClientPacket
{
    public required string Message { get; init; }
}
