using Archean.Core.Exceptions;

namespace Archean.Application.Services.Networking;

public class ServerPacketWriter : IServerPacketWriter
{
    private readonly IPacketDataWriter writer;

    public ServerPacketWriter(IPacketDataWriter writer)
    {
        this.writer = writer;
    }

    public ReadOnlyMemory<byte> WritePacket(IServerPacket packet)
    {
        return packet switch
        {
            ServerAbsolutePositionAndOrientationPacket p => WriteAbsolutePositionAndOrientationPacket(p),
            ServerDespawnPlayerPacket p => WriteDespawnPlayerPacket(p),
            ServerDisconnectPlayerPacket p => WriteDisconnectPlayerPacket(p),
            ServerIdentificationPacket p => WriteIdentificationPacket(p),
            ServerLevelDataChunkPacket p => WriteLevelDataChunkPacket(p),
            ServerLevelFinalizePacket p => WriteLevelFinalizePacket(p),
            ServerLevelInitializePacket p => WriteLevelInitializePacket(p),
            ServerMessagePacket p => WriteMessagePacket(p),
            ServerPingPacket p => WritePingPacket(p),
            ServerRelativeOrientationPacket p => WriteRelativeOrientationPacket(p),
            ServerRelativePositionAndOrientationPacket p => WriteRelativePositionAndOrientationPacket(p),
            ServerRelativePositionPacket p => WriteRelativePositionPacket(p),
            ServerSetBlockPacket p => WriteSetBlockPacket(p),
            ServerSpawnPlayerPacket p => WriteSpawnPlayerPacket(p),
            ServerUpdatePlayerTypePacket p => WriteUpdatePlayerTypePacket(p),
            _ => throw new UnexpectedPacketTypeException(packet.GetType())
        };
    }

    public ReadOnlyMemory<byte> WriteAbsolutePositionAndOrientationPacket(ServerAbsolutePositionAndOrientationPacket packet)
    {
        Memory<byte> buffer = new byte[ServerAbsolutePositionAndOrientationPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.AbsolutePositionAndOrientation, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteFShort(packet.X, restBuffer, out restBuffer);
        writer.WriteFShort(packet.Y, restBuffer, out restBuffer);
        writer.WriteFShort(packet.Z, restBuffer, out restBuffer);
        writer.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        writer.WriteByte(packet.Pitch, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteDespawnPlayerPacket(ServerDespawnPlayerPacket packet)
    {
        Memory<byte> buffer = new byte[ServerDespawnPlayerPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.DespawnPlayer, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteDisconnectPlayerPacket(ServerDisconnectPlayerPacket packet)
    {
        Memory<byte> buffer = new byte[ServerDisconnectPlayerPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.DisconnectPlayer, buffer, out Memory<byte> restBuffer);
        writer.WriteString(packet.Message, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteIdentificationPacket(ServerIdentificationPacket packet)
    {
        Memory<byte> buffer = new byte[ServerIdentificationPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.Identification, buffer, out Memory<byte> restBuffer);
        writer.WriteByte(packet.ProtocolVersion, restBuffer, out restBuffer);
        writer.WriteString(packet.ServerName, restBuffer, out restBuffer);
        writer.WriteString(packet.ServerMotd, restBuffer, out restBuffer);
        writer.WriteByte((byte)packet.PlayerType, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteLevelDataChunkPacket(ServerLevelDataChunkPacket packet)
    {
        Memory<byte> buffer = new byte[ServerLevelDataChunkPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.LevelDataChunk, buffer, out Memory<byte> restBuffer);
        writer.WriteShort(packet.ChunkLength, restBuffer, out restBuffer);
        writer.WriteByteArray(packet.ChunkData, restBuffer, out restBuffer);
        writer.WriteByte(packet.PercentageComplete, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteLevelFinalizePacket(ServerLevelFinalizePacket packet)
    {
        Memory<byte> buffer = new byte[ServerLevelFinalizePacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.LevelFinalize, buffer, out Memory<byte> restBuffer);
        writer.WriteShort(packet.XSize, restBuffer, out restBuffer);
        writer.WriteShort(packet.YSize, restBuffer, out restBuffer);
        writer.WriteShort(packet.ZSize, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteLevelInitializePacket(ServerLevelInitializePacket packet)
    {
        return new byte[]
        {
            (byte)ServerPacketId.LevelInitialize
        };
    }

    public ReadOnlyMemory<byte> WriteMessagePacket(ServerMessagePacket packet)
    {
        Memory<byte> buffer = new byte[ServerMessagePacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.Message, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteString(packet.Message, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WritePingPacket(ServerPingPacket packet)
    {
        return new byte[]
        {
            (byte)ServerPacketId.Ping
        };
    }

    public ReadOnlyMemory<byte> WriteRelativeOrientationPacket(ServerRelativeOrientationPacket packet)
    {
        Memory<byte> buffer = new byte[ServerRelativeOrientationPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.RelativeOrientation, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        writer.WriteByte(packet.Pitch, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteRelativePositionAndOrientationPacket(ServerRelativePositionAndOrientationPacket packet)
    {
        Memory<byte> buffer = new byte[ServerRelativePositionAndOrientationPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.RelativePositionAndOrientation, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteFByte(packet.X, restBuffer, out restBuffer);
        writer.WriteFByte(packet.Y, restBuffer, out restBuffer);
        writer.WriteFByte(packet.Z, restBuffer, out restBuffer);
        writer.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        writer.WriteByte(packet.Pitch, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteRelativePositionPacket(ServerRelativePositionPacket packet)
    {
        Memory<byte> buffer = new byte[ServerRelativePositionPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.RelativePosition, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteFByte(packet.X, restBuffer, out restBuffer);
        writer.WriteFByte(packet.Y, restBuffer, out restBuffer);
        writer.WriteFByte(packet.Z, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteSetBlockPacket(ServerSetBlockPacket packet)
    {
        Memory<byte> buffer = new byte[ServerSetBlockPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.SetBlock, buffer, out Memory<byte> restBuffer);
        writer.WriteShort(packet.X, restBuffer, out restBuffer);
        writer.WriteShort(packet.Y, restBuffer, out restBuffer);
        writer.WriteShort(packet.Z, restBuffer, out restBuffer);
        writer.WriteByte((byte)packet.BlockType, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteSpawnPlayerPacket(ServerSpawnPlayerPacket packet)
    {
        Memory<byte> buffer = new byte[ServerSpawnPlayerPacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.SpawnPlayer, buffer, out Memory<byte> restBuffer);
        writer.WriteSByte(packet.PlayerId, restBuffer, out restBuffer);
        writer.WriteString(packet.PlayerName, restBuffer, out restBuffer);
        writer.WriteFShort(packet.X, restBuffer, out restBuffer);
        writer.WriteFShort(packet.Y, restBuffer, out restBuffer);
        writer.WriteFShort(packet.Z, restBuffer, out restBuffer);
        writer.WriteByte(packet.Yaw, restBuffer, out restBuffer);
        writer.WriteByte(packet.Pitch, restBuffer, out _);

        return buffer;
    }

    public ReadOnlyMemory<byte> WriteUpdatePlayerTypePacket(ServerUpdatePlayerTypePacket packet)
    {
        Memory<byte> buffer = new byte[ServerUpdatePlayerTypePacket.PacketSize];

        writer.WriteByte((byte)ServerPacketId.UpdatePlayerType, buffer, out Memory<byte> restBuffer);
        writer.WriteByte((byte)packet.PlayerType, restBuffer, out _);

        return buffer;
    }
}
