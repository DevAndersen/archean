namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientIdentificationPacket : IClientPacket
{
    /// <summary>
    /// The size of the packet, in bytes.
    /// </summary>
    public const int PacketSize
        = sizeof(ClientPacketId) // ProtocolVersion
        + Constants.Networking.StringLength // Username
        + Constants.Networking.StringLength // VerificationKey
        + sizeof(byte); // Unused

    public ClientPacketId PacketId => ClientPacketId.Identification;

    public required byte ProtocolVersion { get; init; }

    public required string Username { get; init; }

    public required string VerificationKey { get; init; }
}
