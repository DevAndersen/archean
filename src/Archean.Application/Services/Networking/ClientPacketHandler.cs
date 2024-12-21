using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking;
using Archean.Core.Models.Networking.ClientPackets;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientPacketHandler : IClientPacketHandler
{
    private readonly ILogger<ClientPacketHandler> logger;
    private readonly IPlayerService playerService;
    private readonly IGlobalEventBus globalEventBus;
    private readonly IServerPacketWriter serverPacketWriter;

    public ClientPacketHandler(ILogger<ClientPacketHandler> logger, IPlayerService playerService, IGlobalEventBus globalEventBus, IEventListener eventListener, IServerPacketWriter serverPacketWriter)
    {
        this.logger = logger;
        this.playerService = playerService;
        this.globalEventBus = globalEventBus;

        eventListener.Subscribe<MessageEvent>(ReceiveMessage);
        eventListener.Subscribe<SetBlockEvent>(ReceiveSetBlock);
        this.serverPacketWriter = serverPacketWriter;
    }

    private async Task ReceiveMessage(MessageEvent arg)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            ServerMessagePacket packet;

            if (arg.PlayerSender != null)
            {
                packet = new ServerMessagePacket
                {
                    Message = $"{arg.PlayerSender.DisplayName}: {arg.Message}",
                    PlayerId = arg.PlayerSender.Id
                };
            }
            else
            {
                packet = new ServerMessagePacket
                {
                    Message = arg.Message,
                    PlayerId = 0
                };
            }
            await player.Connection.SendAsync(serverPacketWriter.WriteMessagePacket(packet));
        }
    }

    private async Task ReceiveSetBlock(SetBlockEvent arg)
    {
        if (playerService.TryGetPlayer(out IPlayer? player))
        {
            await player.Connection.SendAsync(serverPacketWriter.WriteSetBlockPacket(new ServerSetBlockPacket
            {
                BlockType = arg.Mode == BlockChangeMode.Break ? Block.Air : arg.BlockType,
                X = arg.X,
                Y = arg.Y,
                Z = arg.Z
            }));
        }
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

    public Task HandlePositionAndOrientationPacketAsync(ClientPositionAndOrientationPacket packet)
    {
        //logger.LogInformation("{x}:{y}:{z}", packet.X, packet.Y, packet.Z);
        return Task.CompletedTask;
    }

    public async Task HandleSetBlockPacketAsync(ClientSetBlockPacket packet)
    {
        await globalEventBus.InvokeEventAsync(new SetBlockEvent
        {
            X = packet.X,
            Y = packet.Y,
            Z = packet.Z,
            Mode = packet.Mode,
            BlockType = packet.BlockType,
        });
    }
}
