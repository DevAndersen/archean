using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Core.Services.Networking;

public interface IServerPacketWriter
{
    ReadOnlyMemory<byte> WritePacket(IServerPacket packet);

    ReadOnlyMemory<byte> WriteAbsolutePositionAndOrientationPacket(ServerAbsolutePositionAndOrientationPacket packet);

    ReadOnlyMemory<byte> WriteDespawnPlayerPacket(ServerDespawnPlayerPacket packet);

    ReadOnlyMemory<byte> WriteDisconnectPlayerPacket(ServerDisconnectPlayerPacket packet);

    ReadOnlyMemory<byte> WriteIdentificationPacket(ServerIdentificationPacket packet);

    ReadOnlyMemory<byte> WriteLevelDataChunkPacket(ServerLevelDataChunkPacket packet);

    ReadOnlyMemory<byte> WriteLevelFinalizePacket(ServerLevelFinalizePacket packet);

    ReadOnlyMemory<byte> WriteLevelInitializePacket(ServerLevelInitializePacket packet);

    ReadOnlyMemory<byte> WriteMessagePacket(ServerMessagePacket packet);

    ReadOnlyMemory<byte> WritePingPacket(ServerPingPacket packet);

    ReadOnlyMemory<byte> WriteRelativeOrientationPacket(ServerRelativeOrientationPacket packet);

    ReadOnlyMemory<byte> WriteRelativePositionAndOrientationPacket(ServerRelativePositionAndOrientationPacket packet);

    ReadOnlyMemory<byte> WriteRelativePositionPacket(ServerRelativePositionPacket packet);

    ReadOnlyMemory<byte> WriteSetBlockPacket(ServerSetBlockPacket packet);

    ReadOnlyMemory<byte> WriteSpawnPlayerPacket(ServerSpawnPlayerPacket packet);

    ReadOnlyMemory<byte> WriteUpdatePlayerTypePacket(ServerUpdatePlayerTypePacket packet);
}
