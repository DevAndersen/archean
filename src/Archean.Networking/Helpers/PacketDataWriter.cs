using Archean.Core;
using Archean.Core.Models.Networking;
using System.Buffers.Binary;
using System.Text;

namespace Archean.Networking.Helpers;

public static class PacketDataWriter
{
    public static void WriteByte(byte value, Span<byte> memory, out Span<byte> rest)
    {
        memory[0] = value;
        rest = memory[sizeof(byte)..];
    }

    public static void WriteSByte(sbyte value, Span<byte> memory, out Span<byte> rest)
    {
        memory[0] = (byte)value;
        rest = memory[sizeof(sbyte)..];
    }

    public static void WriteShort(short value, Span<byte> memory, out Span<byte> rest)
    {
        BinaryPrimitives.WriteInt16BigEndian(memory, value);
        rest = memory[sizeof(short)..];
    }

    public static void WriteFByte(FByte value, Span<byte> memory, out Span<byte> rest)
    {
        memory[0] = value.ToByte();
        rest = memory[sizeof(byte)..];
    }

    public static void WriteFShort(FShort value, Span<byte> memory, out Span<byte> rest)
    {
        BinaryPrimitives.WriteUInt16BigEndian(memory, value.ToUShort());
        rest = memory[sizeof(short)..];
    }

    public static void WriteString(string value, Span<byte> memory, out Span<byte> rest)
    {
        if (memory.Length < Constants.Networking.StringLength)
        {
            throw new ArgumentOutOfRangeException(nameof(memory), "Memory buffer too small");
        }

        if (value.Length > Constants.Networking.StringLength)
        {
            Encoding.UTF8.GetBytes(value.AsSpan()[..Constants.Networking.StringLength], memory);
        }
        else
        {
            Encoding.UTF8.GetBytes(value, memory);
            memory[value.Length..Constants.Networking.StringLength].Fill((byte)' ');
        }
        rest = memory[Constants.Networking.StringLength..];
    }

    public static void WriteByteArray(byte[] bytes, Span<byte> memory, out Span<byte> rest)
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
        rest = memory[Constants.Networking.ByteArrayLength..];
    }
}
