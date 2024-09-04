﻿using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketReader : IClientPacketReader
{
    private readonly IPacketDataReader reader;

    public ClientPacketReader(IPacketDataReader reader)
    {
        this.reader = reader;
    }

    public IClientPacket ReadClientPacket(ReadOnlyMemory<byte> buffer, out ReadOnlyMemory<byte> restBuffer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(buffer.Length, nameof(buffer));

        ClientPacketId packetId = (ClientPacketId)reader.ReadByte(buffer, out buffer);

        IClientPacket packet = packetId switch
        {
            ClientPacketId.Identification => ReadIdentificationPacket(buffer),
            ClientPacketId.SetBlock => ReadMessagePacket(buffer),
            ClientPacketId.PositionAndOrientation => ReadPositionAndOrientationPacket(buffer),
            ClientPacketId.Message => ReadSetBlockPacket(buffer),
            _ => throw new NotImplementedException(),
        };

        int packetSize = packet switch
        {
            ClientIdentificationPacket => ClientIdentificationPacket.PacketSize,
            ClientMessagePacket => ClientMessagePacket.PacketSize,
            ClientPositionAndOrientationPacket => ClientPositionAndOrientationPacket.PacketSize,
            ClientSetBlockPacket => ClientSetBlockPacket.PacketSize,
            _ => throw new NotImplementedException(),
        };

        restBuffer = buffer.Slice(packetSize);
        return packet;
    }

    public ClientIdentificationPacket ReadIdentificationPacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientIdentificationPacket.PacketSize, nameof(memory));

        byte protocolVersion = reader.ReadByte(memory, out memory);
        string username = reader.ReadString(memory, out memory);
        string verificationKey = reader.ReadString(memory, out _);

        return new ClientIdentificationPacket
        {
            ProtocolVersion = protocolVersion,
            Username = username,
            VerificationKey = verificationKey
        };
    }

    public ClientMessagePacket ReadMessagePacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientMessagePacket.PacketSize, nameof(memory));

        // Skip the first byte, which represents the player ID (always 0xFF).
        string message = reader.ReadString(memory.Slice(1), out _);

        return new ClientMessagePacket
        {
            Message = message
        };
    }

    public ClientPositionAndOrientationPacket ReadPositionAndOrientationPacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientPositionAndOrientationPacket.PacketSize, nameof(memory));

        // Skip the first byte, which represents the player ID (always 0xFF).
        FShort x = reader.ReadFShort(memory.Slice(1), out memory);
        FShort y = reader.ReadFShort(memory, out memory);
        FShort z = reader.ReadFShort(memory, out memory);
        byte yaw = reader.ReadByte(memory, out memory);
        byte pitch = reader.ReadByte(memory, out _);

        return new ClientPositionAndOrientationPacket
        {
            X = x,
            Y = y,
            Z = z,
            Yaw = yaw,
            Pitch = pitch
        };
    }

    public ClientSetBlockPacket ReadSetBlockPacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientSetBlockPacket.PacketSize, nameof(memory));

        short x = reader.ReadShort(memory, out memory);
        short y = reader.ReadShort(memory, out memory);
        short z = reader.ReadShort(memory, out memory);
        BlockChangeMode mode = (BlockChangeMode)reader.ReadByte(memory, out memory);
        Block blockType = (Block)reader.ReadByte(memory, out _);

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