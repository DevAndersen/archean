using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Core.Services.Networking;

public interface IServerPacketWriter
{
    public ReadOnlyMemory<byte> WritePacket(IServerPacket packet);

    public ReadOnlyMemory<byte> WriteAbsolutePositionAndOrientationPacket(ServerAbsolutePositionAndOrientationPacket packet);

    public ReadOnlyMemory<byte> WriteDespawnPlayerPacket(ServerDespawnPlayerPacket packet);

    public ReadOnlyMemory<byte> WriteDisconnectPlayerPacket(ServerDisconnectPlayerPacket packet);

    public ReadOnlyMemory<byte> WriteIdentificationPacket(ServerIdentificationPacket packet);

    public ReadOnlyMemory<byte> WriteLevelDataChunkPacket(ServerLevelDataChunkPacket packet);

    public ReadOnlyMemory<byte> WriteLevelFinalizePacket(ServerLevelFinalizePacket packet);

    public ReadOnlyMemory<byte> WriteLevelInitializePacket(ServerLevelInitializePacket packet);

    public ReadOnlyMemory<byte> WriteMessagePacket(ServerMessagePacket packet);

    public ReadOnlyMemory<byte> WritePingPacket(ServerPingPacket packet);

    public ReadOnlyMemory<byte> WriteRelativeOrientationPacket(ServerRelativeOrientationPacket packet);

    public ReadOnlyMemory<byte> WriteRelativePositionAndOrientationPacket(ServerRelativePositionAndOrientationPacket packet);

    public ReadOnlyMemory<byte> WriteRelativePositionPacket(ServerRelativePositionPacket packet);

    public ReadOnlyMemory<byte> WriteSetBlockPacket(ServerSetBlockPacket packet);

    public ReadOnlyMemory<byte> WriteSpawnPlayerPacket(ServerSpawnPlayerPacket packet);

    public ReadOnlyMemory<byte> WriteUpdatePlayerTypePacket(ServerUpdatePlayerTypePacket packet);
}
