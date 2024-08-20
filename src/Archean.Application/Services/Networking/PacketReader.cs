using Archean.Application.Models.Networking;
using System.Buffers.Binary;

namespace Archean.Application.Services.Networking;

internal static class PacketReader
{
    public static byte ReadByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes.Slice(sizeof(byte));
        return bytes.Span[0];
    }

    public static sbyte ReadSByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes.Slice(sizeof(sbyte));
        return (sbyte)bytes.Span[0];
    }

    public static short ReadShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        rest = bytes.Slice(sizeof(short));
        return BinaryPrimitives.ReadInt16BigEndian(bytes.Span);
    }

    public static string ReadString(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = Constants.Networking.StringLength;
        rest = bytes.Slice(read);
        return Encoding.ASCII.GetString(bytes.Span.Slice(0, read)).TrimEnd().TrimEnd((char)0);
    }

    public static byte[] ReadByteArray(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = Constants.Networking.ByteArrayLength;
        rest = bytes.Slice(read);
        return bytes.Slice(0, read).ToArray();
    }

    public static FByte ReadFByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(byte);
        rest = bytes.Slice(read);
        return new FByte(bytes.Span[0]);
    }

    public static FShort ReadFShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = bytes.Slice(read);
        return new FShort(BinaryPrimitives.ReadUInt16BigEndian(bytes.Span));
    }

    public static ushort ReadUShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = bytes.Slice(read);
        return BinaryPrimitives.ReadUInt16BigEndian(bytes.Span);
    }
}
