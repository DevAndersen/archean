namespace Archean.Networking.Helpers;

public static class PacketDataWriter
{
    public static void WriteByte(byte value, Span<byte> buffer, out Span<byte> rest)
    {
        buffer[0] = value;
        rest = buffer[sizeof(byte)..];
    }

    public static void WriteSByte(sbyte value, Span<byte> buffer, out Span<byte> rest)
    {
        buffer[0] = (byte)value;
        rest = buffer[sizeof(sbyte)..];
    }

    public static void WriteShort(short value, Span<byte> buffer, out Span<byte> rest)
    {
        BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        rest = buffer[sizeof(short)..];
    }

    public static void WriteFByte(FByte value, Span<byte> buffer, out Span<byte> rest)
    {
        buffer[0] = value.ToByte();
        rest = buffer[sizeof(byte)..];
    }

    public static void WriteFShort(FShort value, Span<byte> buffer, out Span<byte> rest)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, value.ToUShort());
        rest = buffer[sizeof(short)..];
    }

    public static void WriteString(string value, Span<byte> buffer, out Span<byte> rest)
    {
        if (buffer.Length < Constants.Networking.StringLength)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Memory buffer too small");
        }

        if (value.Length > Constants.Networking.StringLength)
        {
            Encoding.UTF8.GetBytes(value.AsSpan()[..Constants.Networking.StringLength], buffer);
        }
        else
        {
            Encoding.UTF8.GetBytes(value, buffer);
            buffer[value.Length..Constants.Networking.StringLength].Fill((byte)' ');
        }
        rest = buffer[Constants.Networking.StringLength..];
    }

    public static void WriteByteArray(byte[] bytes, Span<byte> buffer, out Span<byte> rest)
    {
        if (buffer.Length < Constants.Networking.ByteArrayLength)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Memory buffer too small");
        }

        if (bytes.Length > Constants.Networking.ByteArrayLength)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), "Input buffer too long");
        }

        bytes.CopyTo(buffer);
        rest = buffer[Constants.Networking.ByteArrayLength..];
    }
}
