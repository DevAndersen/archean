namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientMessagePacket : IClientPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(byte) // Player ID, unused
        + Constants.Networking.StringLength; // Message

    public ClientPacketId PacketId => ClientPacketId.Message;

    public required string Message { get; init;  }
}
