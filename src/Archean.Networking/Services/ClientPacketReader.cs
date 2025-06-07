using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Networking;

namespace Archean.Networking.Services;

public class ClientPacketReader : IClientPacketReader
{
    private readonly IPacketDataReader _reader;

    public ClientPacketReader(IPacketDataReader reader)
    {
        _reader = reader;
    }

    public ClientIdentificationPacket ReadIdentificationPacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientIdentificationPacket.PacketSize, nameof(memory));

        byte protocolVersion = _reader.ReadByte(memory, out memory);
        string username = _reader.ReadString(memory, out memory);
        string verificationKey = _reader.ReadString(memory, out _);

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
        string message = _reader.ReadString(memory[1..], out _);

        return new ClientMessagePacket
        {
            Message = message
        };
    }

    public ClientPositionAndOrientationPacket ReadPositionAndOrientationPacket(ReadOnlyMemory<byte> memory)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(memory.Length, ClientPositionAndOrientationPacket.PacketSize, nameof(memory));

        // Skip the first byte, which represents the player ID (always 0xFF).
        FShort x = _reader.ReadFShort(memory[1..], out memory);
        FShort y = _reader.ReadFShort(memory, out memory);
        FShort z = _reader.ReadFShort(memory, out memory);
        byte yaw = _reader.ReadByte(memory, out memory);
        byte pitch = _reader.ReadByte(memory, out _);

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

        short x = _reader.ReadShort(memory, out memory);
        short y = _reader.ReadShort(memory, out memory);
        short z = _reader.ReadShort(memory, out memory);
        BlockChangeMode mode = (BlockChangeMode)_reader.ReadByte(memory, out memory);
        Block blockType = (Block)_reader.ReadByte(memory, out _);

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
