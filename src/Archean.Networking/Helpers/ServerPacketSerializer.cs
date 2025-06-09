namespace Archean.Networking.Helpers;

public static class ServerPacketSerializer
{
    public static void WritePackets(IEnumerable<IServerPacket> packets, Span<byte> buffer)
    {
        foreach (IServerPacket packet in packets)
        {
            int packetSize = ServerPacketSizer.CalculateSize(packet);
            WritePacket(packet, buffer[..packetSize]);
            buffer = buffer[packetSize..];
        }
    }

    private static void WritePacket(IServerPacket packet, Span<byte> buffer)
    {
        switch (packet)
        {
            case ServerAbsolutePositionAndOrientationPacket p:
                WriteAbsolutePositionAndOrientationPacket(p, buffer);
                break;

            case ServerDespawnPlayerPacket p:
                WriteDespawnPlayerPacket(p, buffer);
                break;

            case ServerDisconnectPlayerPacket p:
                WriteDisconnectPlayerPacket(p, buffer);
                break;

            case ServerIdentificationPacket p:
                WriteIdentificationPacket(p, buffer);
                break;

            case ServerLevelDataChunkPacket p:
                WriteLevelDataChunkPacket(p, buffer);
                break;

            case ServerLevelFinalizePacket p:
                WriteLevelFinalizePacket(p, buffer);
                break;

            case ServerLevelInitializePacket:
                WriteLevelInitializePacket(buffer);
                break;

            case ServerMessagePacket p:
                WriteMessagePacket(p, buffer);
                break;

            case ServerPingPacket:
                WritePingPacket(buffer);
                break;

            case ServerRelativeOrientationPacket p:
                WriteRelativeOrientationPacket(p, buffer);
                break;

            case ServerRelativePositionAndOrientationPacket p:
                WriteRelativePositionAndOrientationPacket(p, buffer);
                break;

            case ServerRelativePositionPacket p:
                WriteRelativePositionPacket(p, buffer);
                break;

            case ServerSetBlockPacket p:
                WriteSetBlockPacket(p, buffer);
                break;

            case ServerSpawnPlayerPacket p:
                WriteSpawnPlayerPacket(p, buffer);
                break;

            case ServerUpdatePlayerTypePacket p:
                WriteUpdatePlayerTypePacket(p, buffer);
                break;

            default:
                throw new UnexpectedPacketTypeException(packet.GetType());
        }
    }

    private static void WriteAbsolutePositionAndOrientationPacket(ServerAbsolutePositionAndOrientationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.AbsolutePositionAndOrientation, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.X, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.Y, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.Z, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Yaw, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Pitch, buffer, out _);
    }

    private static void WriteDespawnPlayerPacket(ServerDespawnPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.DespawnPlayer, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out _);
    }

    private static void WriteDisconnectPlayerPacket(ServerDisconnectPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.DisconnectPlayer, buffer, out buffer);
        PacketDataWriter.WriteString(packet.Message, buffer, out _);
    }

    private static void WriteIdentificationPacket(ServerIdentificationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Identification, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.ProtocolVersion, buffer, out buffer);
        PacketDataWriter.WriteString(packet.ServerName, buffer, out buffer);
        PacketDataWriter.WriteString(packet.ServerMotd, buffer, out buffer);
        PacketDataWriter.WriteByte((byte)packet.PlayerType, buffer, out _);
    }

    private static void WriteLevelDataChunkPacket(ServerLevelDataChunkPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelDataChunk, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.ChunkLength, buffer, out buffer);
        PacketDataWriter.WriteByteArray(packet.ChunkData, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.PercentageComplete, buffer, out _);
    }

    private static void WriteLevelFinalizePacket(ServerLevelFinalizePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelFinalize, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.XSize, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.YSize, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.ZSize, buffer, out _);
    }

    private static void WriteLevelInitializePacket(Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelInitialize, buffer, out _);
    }

    private static void WriteMessagePacket(ServerMessagePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Message, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteString(packet.Message, buffer, out _);
    }

    private static void WritePingPacket(Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Ping, buffer, out _);
    }

    private static void WriteRelativeOrientationPacket(ServerRelativeOrientationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativeOrientation, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Yaw, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Pitch, buffer, out _);
    }

    private static void WriteRelativePositionAndOrientationPacket(ServerRelativePositionAndOrientationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativePositionAndOrientation, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.X, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.Y, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.Z, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Yaw, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Pitch, buffer, out _);
    }

    private static void WriteRelativePositionPacket(ServerRelativePositionPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativePosition, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.X, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.Y, buffer, out buffer);
        PacketDataWriter.WriteFByte(packet.Z, buffer, out _);
    }

    private static void WriteSetBlockPacket(ServerSetBlockPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.SetBlock, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.X, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.Y, buffer, out buffer);
        PacketDataWriter.WriteShort(packet.Z, buffer, out buffer);
        PacketDataWriter.WriteByte((byte)packet.BlockType, buffer, out _);
    }

    private static void WriteSpawnPlayerPacket(ServerSpawnPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.SpawnPlayer, buffer, out buffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, buffer, out buffer);
        PacketDataWriter.WriteString(packet.PlayerName, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.X, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.Y, buffer, out buffer);
        PacketDataWriter.WriteFShort(packet.Z, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Yaw, buffer, out buffer);
        PacketDataWriter.WriteByte(packet.Pitch, buffer, out _);
    }

    private static void WriteUpdatePlayerTypePacket(ServerUpdatePlayerTypePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.UpdatePlayerType, buffer, out buffer);
        PacketDataWriter.WriteByte((byte)packet.PlayerType, buffer, out _);
    }
}
