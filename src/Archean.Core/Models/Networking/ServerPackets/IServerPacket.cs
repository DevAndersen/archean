namespace Archean.Core.Models.Networking.ServerPackets;

public interface IServerPacket
{
    ServerPacketId PacketId { get; }
}
