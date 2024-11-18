using Archean.Core.Models.Networking.ClientPackets;

namespace Archean.Core.Services.Networking;

public interface IClientPacketHandler
{
    public Task HandleMessagePacketAsync(ClientMessagePacket packet);

    public Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet);

    public Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet);
}
