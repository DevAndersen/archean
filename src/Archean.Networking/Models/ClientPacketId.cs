﻿namespace Archean.Networking.Models;

/// <summary>
/// Client packet IDs.
/// </summary>
public enum ClientPacketId : byte
{
    Identification = 0,
    SetBlock = 5,
    PositionAndOrientation = 8,
    Message = 13
}
