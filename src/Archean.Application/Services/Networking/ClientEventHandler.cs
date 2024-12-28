using Archean.Application.Settings;
using Microsoft.Extensions.Options;

namespace Archean.Application.Services.Networking;

public class ClientEventHandler : IClientEventHandler
{
    private readonly IEventListener eventListener;
    private readonly IPlayerService playerService;
    private readonly ChatSettings chatSettings;

    public ClientEventHandler(
        IEventListener eventListener,
        IPlayerService playerService,
        IOptions<ChatSettings> chatSettingsOptions)
    {
        this.eventListener = eventListener;
        this.playerService = playerService;
        chatSettings = chatSettingsOptions.Value;
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
                    Message = string.Format(chatSettings.ChatFormat, arg.Message, arg.PlayerSender.DisplayName),
                    PlayerId = arg.PlayerSender.Id
                };
            }
            else
            {
                packet = new ServerMessagePacket
                {
                    Message = string.Format(chatSettings.ServerChatFormat, arg.Message),
                    PlayerId = 0
                };
            }
            await player.Connection.SendAsync(packet);
        }
    }
}
