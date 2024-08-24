namespace Archean.Core.Models.Networking.ServerPackets;

public interface IServerPacket
{
    public ServerPacketId PacketId { get; }
}
