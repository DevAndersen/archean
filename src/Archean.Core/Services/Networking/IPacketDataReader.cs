using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPacketDataReader
{
    byte ReadByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    byte[] ReadByteArray(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    FByte ReadFByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    FShort ReadFShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    sbyte ReadSByte(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    short ReadShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    string ReadString(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);

    ushort ReadUShort(ReadOnlyMemory<byte> bytes, out ReadOnlyMemory<byte> rest);
}
