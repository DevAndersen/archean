namespace Archean.Networking.Helpers;

public static class ClientPacketSizer
{
    public const int ClientIdentificationPacketSize
        = sizeof(ClientPacketId) // ProtocolVersion
        + Constants.Networking.StringLength // Username
        + Constants.Networking.StringLength // VerificationKey
        + sizeof(byte); // Unused

    public const int ClientMessagePacketSize
        = sizeof(byte) // Player ID, unused
        + Constants.Networking.StringLength; // Message

    public const int ClientPositionAndOrientationPacketSize
        = sizeof(byte) // Player ID, unused
        + sizeof(short) // X
        + sizeof(short) // Y
        + sizeof(short) // Z
        + sizeof(byte) // YaW
        + sizeof(byte); // Pitch

    public const int ClientSetBlockPacketSize
        = sizeof(short) // X
        + sizeof(short) // Y
        + sizeof(short) // Z
        + sizeof(BlockChangeMode) // Mode
        + sizeof(Block); // Block
}
