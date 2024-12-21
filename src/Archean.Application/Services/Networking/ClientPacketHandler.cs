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
    private readonly IPlayerService playerService;
    private readonly IGlobalEventBus globalEventBus;
    private readonly IServerPacketWriter serverPacketWriter;

    public ClientPacketHandler(
        IPlayerService playerService,
        IGlobalEventBus globalEventBus,
        IServerPacketWriter serverPacketWriter,
        IEventListener eventListener)
    {
        this.playerService = playerService;
        this.globalEventBus = globalEventBus;
        this.serverPacketWriter = serverPacketWriter;

        eventListener.Subscribe<MessageEvent>(ReceiveMessage);
        eventListener.Subscribe<SetBlockEvent>(ReceiveSetBlock);
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
