using Archean.Core;
using Archean.Core.Exceptions;
using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ServerPackets;

namespace Archean.Networking.Helpers;

public static class ServerPacketSizer
{
    public const int ServerAbsolutePositionAndOrientationPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FShort.Size // X
        + FShort.Size // Y
        + FShort.Size // Z
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public const int ServerDespawnPlayerPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte); // Player ID

    public const int ServerDisconnectPlayerPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + Constants.Networking.StringLength; // Message

    public const int ServerIdentificationPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(byte) // ProtocolVersion
        + Constants.Networking.StringLength // ServerName
        + Constants.Networking.StringLength // ServerMotd
        + sizeof(PlayerType); // PlayerType

    public const int ServerLevelDataChunkPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // ChunkLength
        + Constants.Networking.ByteArrayLength // ChunkData
        + sizeof(byte); // PercentageComplete

    public const int ServerLevelFinalizePacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // XSize
        + sizeof(short) // YSize
        + sizeof(short); // ZSize

    public const int ServerLevelInitializePacketSize
        = sizeof(ServerPacketId); // Packet ID

    public const int ServerMessagePacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + Constants.Networking.StringLength; // Message

    public const int ServerPingPacketSize
        = sizeof(ServerPacketId); // Packet ID

    public const int ServerRelativeOrientationPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public const int ServerRelativePositionAndOrientationPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FByte.Size // X
        + FByte.Size // Y
        + FByte.Size // Z
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public const int ServerRelativePositionPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + FByte.Size // X
        + FByte.Size // Y
        + FByte.Size; // Z

    public const int ServerSetBlockPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(short) // X
        + sizeof(short) // Y
        + sizeof(short) // Z
        + sizeof(Block); // Block

    public const int ServerSpawnPlayerPacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(sbyte) // Player ID
        + Constants.Networking.StringLength // Player name
        + FShort.Size // X
        + FShort.Size // Y
        + FShort.Size // Z
        + sizeof(byte) // Yaw
        + sizeof(byte); // Pitch

    public const int ServerUpdatePlayerTypePacketSize
        = sizeof(ServerPacketId) // Packet ID
        + sizeof(PlayerType); // Player type

    public static int CalculateSize(params IEnumerable<IServerPacket> packets)
    {
        int size = 0;

        foreach (IServerPacket packet in packets)
        {
            size += packet switch
            {
                ServerAbsolutePositionAndOrientationPacket p => ServerAbsolutePositionAndOrientationPacketSize,
                ServerDespawnPlayerPacket p => ServerDespawnPlayerPacketSize,
                ServerDisconnectPlayerPacket p => ServerDisconnectPlayerPacketSize,
                ServerIdentificationPacket p => ServerIdentificationPacketSize,
                ServerLevelDataChunkPacket p => ServerLevelDataChunkPacketSize,
                ServerLevelFinalizePacket p => ServerLevelFinalizePacketSize,
                ServerLevelInitializePacket => ServerLevelInitializePacketSize,
                ServerMessagePacket p => ServerMessagePacketSize,
                ServerPingPacket => ServerPingPacketSize,
                ServerRelativeOrientationPacket p => ServerRelativeOrientationPacketSize,
                ServerRelativePositionAndOrientationPacket p => ServerRelativePositionAndOrientationPacketSize,
                ServerRelativePositionPacket p => ServerRelativePositionPacketSize,
                ServerSetBlockPacket p => ServerSetBlockPacketSize,
                ServerSpawnPlayerPacket p => ServerSpawnPlayerPacketSize,
                ServerUpdatePlayerTypePacket p => ServerUpdatePlayerTypePacketSize,
                _ => throw new UnexpectedPacketTypeException(packet.GetType()),
            };
        }

        return size;
    }
}
