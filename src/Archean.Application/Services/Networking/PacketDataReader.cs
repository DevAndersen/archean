using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;
using System.Buffers.Binary;

namespace Archean.Application.Services.Networking;

public class PacketDataReader : IPacketDataReader
{
    public byte ReadByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes[sizeof(byte)..];
        return bytes.Span[0];
    }

    public sbyte ReadSByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes[sizeof(sbyte)..];
        return (sbyte)bytes.Span[0];
    }

    public short ReadShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes[sizeof(short)..];
        return BinaryPrimitives.ReadInt16BigEndian(bytes.Span);
    }

    public string ReadString(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = Constants.Networking.StringLength;
        rest = bytes[read..];
        return Encoding.ASCII.GetString(bytes.Span[..read]).TrimEnd().TrimEnd((char)0);
    }

    public byte[] ReadByteArray(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = Constants.Networking.ByteArrayLength;
        rest = bytes[read..];
        return bytes[..read].ToArray();
    }

    public FByte ReadFByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(byte);
        rest = bytes[read..];
        return new FByte(bytes.Span[0]);
    }

    public FShort ReadFShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = bytes[read..];
        return new FShort(BinaryPrimitives.ReadUInt16BigEndian(bytes.Span));
    }

    public ushort ReadUShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = bytes[read..];
        return BinaryPrimitives.ReadUInt16BigEndian(bytes.Span);
    }
}
