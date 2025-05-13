using Archean.Core.Models.Networking.ClientPackets;

namespace Archean.Core.Services.Networking;

public interface IClientPacketHandler
{
    Task HandleMessagePacketAsync(ClientMessagePacket packet);

    Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet);

    Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet);
}
