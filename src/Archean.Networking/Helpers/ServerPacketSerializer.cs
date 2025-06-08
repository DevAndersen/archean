using Archean.Core.Exceptions;
using Archean.Core.Models.Networking.ServerPackets;

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
        PacketDataWriter.WriteByte((byte)ServerPacketId.AbsolutePositionAndOrientation, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.X, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.Y, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.Z, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Pitch, restBuffer, out _);
    }

    private static void WriteDespawnPlayerPacket(ServerDespawnPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.DespawnPlayer, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out _);
    }

    private static void WriteDisconnectPlayerPacket(ServerDisconnectPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.DisconnectPlayer, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteString(packet.Message, restBuffer, out _);
    }

    private static void WriteIdentificationPacket(ServerIdentificationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Identification, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteByte(packet.ProtocolVersion, restBuffer, out restBuffer);
        PacketDataWriter.WriteString(packet.ServerName, restBuffer, out restBuffer);
        PacketDataWriter.WriteString(packet.ServerMotd, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte((byte)packet.PlayerType, restBuffer, out _);
    }

    private static void WriteLevelDataChunkPacket(ServerLevelDataChunkPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelDataChunk, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteShort(packet.ChunkLength, restBuffer, out restBuffer);
        PacketDataWriter.WriteByteArray(packet.ChunkData, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.PercentageComplete, restBuffer, out _);
    }

    private static void WriteLevelFinalizePacket(ServerLevelFinalizePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelFinalize, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteShort(packet.XSize, restBuffer, out restBuffer);
        PacketDataWriter.WriteShort(packet.YSize, restBuffer, out restBuffer);
        PacketDataWriter.WriteShort(packet.ZSize, restBuffer, out _);
    }

    private static void WriteLevelInitializePacket(Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.LevelInitialize, buffer, out _);
    }

    private static void WriteMessagePacket(ServerMessagePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Message, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteString(packet.Message, restBuffer, out _);
    }

    private static void WritePingPacket(Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.Ping, buffer, out _);
    }

    private static void WriteRelativeOrientationPacket(ServerRelativeOrientationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativeOrientation, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Pitch, restBuffer, out _);
    }

    private static void WriteRelativePositionAndOrientationPacket(ServerRelativePositionAndOrientationPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativePositionAndOrientation, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.X, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.Y, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.Z, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Pitch, restBuffer, out _);
    }

    private static void WriteRelativePositionPacket(ServerRelativePositionPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.RelativePosition, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.X, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.Y, restBuffer, out restBuffer);
        PacketDataWriter.WriteFByte(packet.Z, restBuffer, out _);
    }

    private static void WriteSetBlockPacket(ServerSetBlockPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.SetBlock, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteShort(packet.X, restBuffer, out restBuffer);
        PacketDataWriter.WriteShort(packet.Y, restBuffer, out restBuffer);
        PacketDataWriter.WriteShort(packet.Z, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte((byte)packet.BlockType, restBuffer, out _);
    }

    private static void WriteSpawnPlayerPacket(ServerSpawnPlayerPacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.SpawnPlayer, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        PacketDataWriter.WriteString(packet.PlayerName, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.X, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.Y, restBuffer, out restBuffer);
        PacketDataWriter.WriteFShort(packet.Z, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        PacketDataWriter.WriteByte(packet.Pitch, restBuffer, out _);
    }

    private static void WriteUpdatePlayerTypePacket(ServerUpdatePlayerTypePacket packet, Span<byte> buffer)
    {
        PacketDataWriter.WriteByte((byte)ServerPacketId.UpdatePlayerType, buffer, out Span<byte> restBuffer);
        PacketDataWriter.WriteByte((byte)packet.PlayerType, restBuffer, out _);
    }
}
