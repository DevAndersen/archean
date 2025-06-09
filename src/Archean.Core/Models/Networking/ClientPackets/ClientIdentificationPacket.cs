namespace Archean.Core.Models.Networking.ClientPackets;

public class ClientIdentificationPacket : IClientPacket
{
    public required byte ProtocolVersion { get; init; }

    public required string Username { get; init; }

    public required string VerificationKey { get; init; }
}
