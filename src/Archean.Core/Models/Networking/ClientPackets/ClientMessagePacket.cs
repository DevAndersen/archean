namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientMessagePacket : IClientPacket
{
    public ClientPacketId PacketId => ClientPacketId.Message;

    public required string Message { get; init; }
}
