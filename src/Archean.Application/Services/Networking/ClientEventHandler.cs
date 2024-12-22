using Archean.Core.Models;
using Archean.Core.Models.Events;
using Archean.Core.Models.Networking.ServerPackets;
using Archean.Core.Services.Events;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ClientEventHandler : IClientEventHandler
{
    private readonly IEventListener eventListener;
    private readonly IPlayerService playerService;
    private readonly IServerPacketWriter serverPacketWriter;

    public ClientEventHandler(
        IEventListener eventListener,
        IPlayerService playerService,
        IServerPacketWriter serverPacketWriter)
    {
        this.eventListener = eventListener;
        this.playerService = playerService;
        this.serverPacketWriter = serverPacketWriter;
    }

    public void RegisterEventSubscriptions()
    {
        eventListener.Subscribe<MessageEvent>(ReceiveMessage);
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
}
