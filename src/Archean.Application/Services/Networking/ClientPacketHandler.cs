using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly ILogger<ClientPacketHandler> logger;

    public ClientPacketHandler(ILogger<ClientPacketHandler> logger)
    {
        this.logger = logger;
    }

    public Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        logger.LogInformation("'{message}'", packet.Message);
        return Task.CompletedTask;
    }

    public Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        logger.LogInformation("{x}:{y}:{z}", packet.X, packet.Y, packet.Z);
        return Task.CompletedTask;
    }

    public Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        logger.LogInformation("{block}", packet.BlockType);
        return Task.CompletedTask;
    }
}
