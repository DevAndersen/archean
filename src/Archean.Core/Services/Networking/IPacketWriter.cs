﻿using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPacketWriter
{
    public void WriteByte(byte value, Memory<byte> memory, out Memory<byte> rest);

    public void WriteByteArray(byte[] bytes, Memory<byte> memory, out Memory<byte> rest);

    public void WriteFByte(FByte value, Memory<byte> memory, out Memory<byte> rest);

    public void WriteFShort(FShort value, Memory<byte> memory, out Memory<byte> rest);

    public void WriteSByte(sbyte value, Memory<byte> memory, out Memory<byte> rest);

    public void WriteShort(short value, Memory<byte> memory, out Memory<byte> rest);

    public void WriteString(string value, Memory<byte> memory, out Memory<byte> rest);
}
