using Archean.Core.Models.Networking.ClientPackets;

namespace Archean.Core.Services.Networking;

public interface IClientPacketReader
{
    ClientIdentificationPacket ReadIdentificationPacket(ReadOnlyMemory<byte> memory);

    ClientMessagePacket ReadMessagePacket(ReadOnlyMemory<byte> memory);

    ClientPositionAndOrientationPacket ReadPositionAndOrientationPacket(ReadOnlyMemory<byte> memory);

    ClientSetBlockPacket ReadSetBlockPacket(ReadOnlyMemory<byte> memory);
}
