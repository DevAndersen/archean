using Archean.Core.Models.Networking.ClientPackets;

namespace Archean.Core.Services.Networking;

public interface IClientPacketReader
{
    public ClientIdentificationPacket ReadIdentificationPacket(ReadOnlyMemory<byte> memory);

    public ClientMessagePacket ReadMessagePacket(ReadOnlyMemory<byte> memory);

    public ClientPositionAndOrientationPacket ReadPositionAndOrientationPacket(ReadOnlyMemory<byte> memory);

    public ClientSetBlockPacket ReadSetBlockPacket(ReadOnlyMemory<byte> memory);
}
