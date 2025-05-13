namespace Archean.Core.Models.Networking.ClientPackets;

public interface IClientPacket
{
    ClientPacketId PacketId { get; }
}
