using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly IPlayerService playerService;
    private readonly IGlobalEventBus globalEventBus;

    public ClientPacketHandler(
        IPlayerService playerService,
        IGlobalEventBus globalEventBus)
    {
        this.playerService = playerService;
        this.globalEventBus = globalEventBus;
    }

    public async Task HandleMessagePacketAsync(ClientMessagePacket packet)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await globalEventBus.InvokeEventAsync(new MessageEvent
            {
                PlayerSender = player,
                Message = packet.Message
            });
        }
    }

    public async Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await globalEventBus.InvokeEventAsync(new PositionAndOrientationEvent
            {
                Player = player,
                X = packet.X.ToFloat(),
                Y = packet.Y.ToFloat(),
                Z = packet.Z.ToFloat(),
                Yaw = packet.Yaw,
                Pitch = packet.Pitch
            });
        }
    }

    public async Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await globalEventBus.InvokeEventAsync(new SetBlockEvent
            {
                Player = player,
                X = packet.X,
                Y = packet.Y,
                Z = packet.Z,
                Mode = packet.Mode,
                BlockType = packet.BlockType,
            });
        }
    }
}
