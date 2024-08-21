namespace Archean.Core.Models.Networking.ServerPackets;

/// <summary>
/// Server packet IDs.
/// </summary>
public enum ServerPacketId : byte
{
    Identification = 0,
    Ping = 1,
    LevelInitialize = 2,
    LevelDataChunk = 3,
    LevelFinalize = 4,
    SetBlock = 6,
    SpawnPlayer = 7,
    AbsolutePositionAndOrientation = 8,
    RelativePositionAndOrientation = 9,
    RelativePosition = 10,
    RelativeOrientation = 11,
    DespawnPlayer = 12,
    Message = 13,
    DisconnectPlayer = 14,
    UpdatePlayerType = 15
}
