using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPacketDataWriter
{
    void WriteByte(byte value, Memory<byte> memory, out Memory<byte> rest);

    void WriteByteArray(byte[] bytes, Memory<byte> memory, out Memory<byte> rest);

    void WriteFByte(FByte value, Memory<byte> memory, out Memory<byte> rest);

    void WriteFShort(FShort value, Memory<byte> memory, out Memory<byte> rest);

    void WriteSByte(sbyte value, Memory<byte> memory, out Memory<byte> rest);

    void WriteShort(short value, Memory<byte> memory, out Memory<byte> rest);

    void WriteString(string value, Memory<byte> memory, out Memory<byte> rest);
}
