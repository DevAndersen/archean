using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPacketDataReader
{
    public byte ReadByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public byte[] ReadByteArray(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public FByte ReadFByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public FShort ReadFShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public sbyte ReadSByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public short ReadShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public string ReadString(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    public ushort ReadUShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);
}
