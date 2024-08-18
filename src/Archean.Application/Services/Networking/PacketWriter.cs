using System.Buffers.Binary;
using Archean.Application.Models.Networking;

namespace Archean.Application.Services.Networking;

internal static class PacketWriter
{
    public static void WriteByte(byte value, Memory<byte> memory, out Memory<byte> rest)
    {
        memory.Span[0] = value;
        rest = memory.Slice(sizeof(byte));
    }

    public static void WriteSByte(sbyte value, Memory<byte> memory, out Memory<byte> rest)
    {
        memory.Span[0] = (byte)value;
        rest = memory.Slice(sizeof(sbyte));
    }

    public static void WriteShort(short value, Memory<byte> memory, out Memory<byte> rest)
    {
        BinaryPrimitives.WriteInt16BigEndian(memory.Span, value);
        rest = memory.Slice(sizeof(short));
    }

    public static void WriteFByte(FByte value, Memory<byte> memory, out Memory<byte> rest)
    {
        memory.Span[0] = value.ToByte();
        rest = memory.Slice(sizeof(byte));
    }

    public static void WriteFShort(FShort value, Memory<byte> memory, out Memory<byte> rest)
    {
        BinaryPrimitives.WriteUInt16BigEndian(memory.Span, value.ToUShort());
        rest = memory.Slice(sizeof(short));
    }

    public static void WriteString(string value, Memory<byte> memory, out Memory<byte> rest)
    {
        if (memory.Length < Constants.Networking.StringLength)
        {
            throw new ArgumentOutOfRangeException(nameof(memory), "Memory buffer too small");
        }

        if (value.Length > Constants.Networking.StringLength)
        {
            Encoding.ASCII.GetBytes(value.AsSpan().Slice(0, Constants.Networking.StringLength), memory.Span);
        }
        else
        {
            Encoding.ASCII.GetBytes(value, memory.Span);
            memory.Slice(value.Length, Constants.Networking.StringLength - value.Length).Span.Fill((byte)' ');
        }
        rest = memory.Slice(Constants.Networking.StringLength);
    }

    public static void WriteByteArray(byte[] bytes, Memory<byte> memory, out Memory<byte> rest)
    {
        if (memory.Length < Constants.Networking.ByteArrayLength)
        {
            throw new ArgumentOutOfRangeException(nameof(memory), "Memory buffer too small");
        }

        if (bytes.Length > Constants.Networking.ByteArrayLength)
        {
            throw new ArgumentOutOfRangeException(nameof(memory), "Input buffer too long");
        }

        bytes.CopyTo(memory);
        rest = memory.Slice(Constants.Networking.ByteArrayLength);
    }
}
