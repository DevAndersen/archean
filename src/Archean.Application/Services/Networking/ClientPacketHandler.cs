using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly ILogger<ClientPacketHandler> logger;
    private readonly IConnectionService connectionService;

    public ClientPacketHandler(ILogger<ClientPacketHandler> logger, IConnectionService connectionService)
    {
        this.logger = logger;
        this.connectionService = connectionService;
    }

    public Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        logger.LogInformation("{identity} '{message}'",
            connectionService.GetConnection()?.ToString(),
            packet.Message);

        return Task.CompletedTask;
    }

    public Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        //logger.LogInformation("{x}:{y}:{z}", packet.X, packet.Y, packet.Z);
        return Task.CompletedTask;
    }

    public Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        //logger.LogInformation("{block}", packet.BlockType);
        return Task.CompletedTask;
    }
}
