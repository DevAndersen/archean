using Archean.Core.Models.Commands;
using Archean.Core.Services.Commands;

namespace Archean.Application.Services.Networking;

public class ClientEventHandler : IClientEventHandler
{
    private readonly IScopedEventListener eventListener;
    private readonly IPlayerService playerService;
    private readonly ChatSettings chatSettings;
    private readonly ICommandDictionary commandDictionary;

    public ClientEventHandler(
        IScopedEventListener eventListener,
        IPlayerService playerService,
        IOptions<ChatSettings> chatSettingsOptions,
        ICommandDictionary commandDictionary)
    {
        this.eventListener = eventListener;
        this.playerService = playerService;
        chatSettings = chatSettingsOptions.Value;
        this.commandDictionary = commandDictionary;
    }

    public void RegisterEventSubscriptions()
    {
        eventListener.Subscribe<MessageEvent>(ReceiveMessage);
    }

    private async Task ReceiveMessage(MessageEvent arg)
    {
        if (arg.Message.StartsWith('/'))
        {
            int spaceIndex = arg.Message.IndexOf(' ');
            ReadOnlySpan<char> nonDigitsOnlyShortest = spaceIndex == -1
                ? arg.Message[1..]
                : arg.Message[1..spaceIndex];

            if (commandDictionary.TryGetCommand(nonDigitsOnlyShortest, out ICommand? command))
            {
                await command.InvokeAsync();
            }
            else if (playerService.TryGetPlayer(out IPlayer? player))
            {
                await player.Connection.SendAsync(new ServerMessagePacket
                {
                    Message = "No such command was found",
                    PlayerId = 0
                });
            }
        }
        else if (playerService.TryGetPlayer(out IPlayer? player))
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
