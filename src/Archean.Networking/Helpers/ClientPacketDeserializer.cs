namespace Archean.Networking.Helpers;

public static class ClientPacketDeserializer
{
    public static IEnumerable<IClientPacket> ReadPackets(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !buffer.IsEmpty)
        {
            yield return ReadPacket(buffer, out buffer);
        }
    }

    private static IClientPacket ReadPacket(ReadOnlyMemory<byte> buffer, out ReadOnlyMemory<byte> rest)
    {
        ClientPacketId packetId = (ClientPacketId)buffer.Span[0];
        // Slice the packet ID byte off.
        buffer = buffer[1..];

        IClientPacket packet;

        switch (packetId)
        {
            // Client identification packet.
            case ClientPacketId.Identification:
                packet = ReadIdentificationPacket(buffer.Span[..ClientPacketSizer.ClientIdentificationPacketSize]);
                rest = buffer[ClientPacketSizer.ClientIdentificationPacketSize..];
                break;

            // Block update packet.
            case ClientPacketId.SetBlock:
                packet = ReadSetBlockPacket(buffer.Span[..ClientPacketSizer.ClientSetBlockPacketSize]);
                rest = buffer[ClientPacketSizer.ClientSetBlockPacketSize..];
                break;

            // Pose packet.
            case ClientPacketId.PositionAndOrientation:
                packet = ReadPositionAndOrientationPacket(buffer.Span[..ClientPacketSizer.ClientPositionAndOrientationPacketSize]);
                rest = buffer[ClientPacketSizer.ClientPositionAndOrientationPacketSize..];
                break;

            // Message packet.
            case ClientPacketId.Message:
                packet = ReadMessagePacket(buffer.Span[..ClientPacketSizer.ClientMessagePacketSize]);
                rest = buffer[ClientPacketSizer.ClientMessagePacketSize..];
                break;

            // Unknown packet type.
            default:
                throw new NotImplementedException(); // Todo: Define an exception type for this.
        }

        return packet;
    }

    public static ClientIdentificationPacket ReadIdentificationPacket(ReadOnlySpan<byte> buffer)
    {
        byte protocolVersion = PacketDataReader.ReadByte(buffer, out buffer);
        string username = PacketDataReader.ReadString(buffer, out buffer);
        string verificationKey = PacketDataReader.ReadString(buffer, out _);

        return new ClientIdentificationPacket
        {
            ProtocolVersion = protocolVersion,
            Username = username,
            VerificationKey = verificationKey
        };
    }

    public static ClientMessagePacket ReadMessagePacket(ReadOnlySpan<byte> buffer)
    {
        // Skip the first byte, which represents the player ID (always 0xFF).
        string message = PacketDataReader.ReadString(buffer[1..], out _);

        return new ClientMessagePacket
        {
            Message = message
        };
    }

    public static ClientPositionAndOrientationPacket ReadPositionAndOrientationPacket(ReadOnlySpan<byte> buffer)
    {
        // Skip the first byte, which represents the player ID (always 0xFF).
        float x = PacketDataReader.ReadFShort(buffer[1..], out buffer);
        float y = PacketDataReader.ReadFShort(buffer, out buffer);
        float z = PacketDataReader.ReadFShort(buffer, out buffer);
        byte yaw = PacketDataReader.ReadByte(buffer, out buffer);
        byte pitch = PacketDataReader.ReadByte(buffer, out _);

        return new ClientPositionAndOrientationPacket
        {
            X = x,
            Y = y,
            Z = z,
            Yaw = yaw,
            Pitch = pitch
        };
    }

    public static ClientSetBlockPacket ReadSetBlockPacket(ReadOnlySpan<byte> buffer)
    {
        short x = PacketDataReader.ReadShort(buffer, out buffer);
        short y = PacketDataReader.ReadShort(buffer, out buffer);
        short z = PacketDataReader.ReadShort(buffer, out buffer);
        BlockChangeMode mode = (BlockChangeMode)PacketDataReader.ReadByte(buffer, out buffer);
        Block blockType = (Block)PacketDataReader.ReadByte(buffer, out _);

        return new ClientSetBlockPacket
        {
            X = x,
            Y = y,
            Z = z,
            Mode = mode,
            BlockType = blockType
        };
    }
}
