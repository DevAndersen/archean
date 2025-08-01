﻿namespace Archean.Networking.Helpers;

public static class PacketDataReader
{
    public static byte ReadByte(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        rest = buffer[sizeof(byte)..];
        return buffer[0];
    }

    public static sbyte ReadSByte(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        rest = buffer[sizeof(sbyte)..];
        return (sbyte)buffer[0];
    }

    public static short ReadShort(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        rest = buffer[sizeof(short)..];
        return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    public static string ReadString(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        const int read = Constants.Networking.StringLength;
        rest = buffer[read..];
        return Encoding.UTF8.GetString(buffer[..read]).TrimEnd().TrimEnd((char)0);
    }

    public static byte[] ReadByteArray(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        const int read = Constants.Networking.ByteArrayLength;
        rest = buffer[read..];
        return buffer[..read].ToArray();
    }

    public static float ReadFByte(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        const int read = sizeof(byte);
        rest = buffer[read..];
        return FixedPointHelper.ReadFixedSByte((sbyte)buffer[0]);
    }

    public static float ReadFShort(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = buffer[read..];
        return FixedPointHelper.ReadFixedShort(BinaryPrimitives.ReadInt16BigEndian(buffer));
    }

    public static ushort ReadUShort(ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> rest)
    {
        const int read = sizeof(ushort);
        rest = buffer[read..];
        return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }
}
