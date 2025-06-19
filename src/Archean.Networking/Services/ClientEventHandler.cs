using Archean.Core.Services.Commands;

namespace Archean.Networking.Services;

public class ClientEventHandler : IClientEventHandler
{
    private readonly IScopedEventListener _eventListener;
    private readonly IPlayerService _playerService;
    private readonly ChatSettings _chatSettings;
    private readonly ICommandInvoker _commandInvoker;
    private readonly ICommandRegistry _commandRegistry;

    public ClientEventHandler(
        IScopedEventListener eventListener,
        IPlayerService playerService,
        IOptions<ChatSettings> chatSettingsOptions,
        ICommandInvoker commandInvoker,
        ICommandRegistry commandRegistry)
    {
        _eventListener = eventListener;
        _playerService = playerService;
        _chatSettings = chatSettingsOptions.Value;
        _commandInvoker = commandInvoker;
        _commandRegistry = commandRegistry;
    }

    public void RegisterEventSubscriptions()
    {
        _eventListener.Subscribe<MessageEvent>(ReceiveMessage);
    }

    private async Task ReceiveMessage(MessageEvent arg)
    {
        if (arg.Message.StartsWith('/'))
        {
            ReadOnlyMemory<char> commandText = arg.Message.AsMemory()[1..];

            if (await _commandInvoker.TryInvokeCommandAsync(commandText))
            {
                if (_playerService.TryGetPlayer(out IPlayer? player))
                {
                    await player.Connection.SendAsync(new ServerMessagePacket
                    {
                        Message = "No such command was found",
                        PlayerId = 0
                    });
                }
            }
        }
        else if (_playerService.TryGetPlayer(out IPlayer? player))
        {
            ServerMessagePacket packet;

            if (arg.PlayerSender != null)
            {
                packet = new ServerMessagePacket
                {
                    Message = string.Format(_chatSettings.ChatFormat, arg.Message, arg.PlayerSender.DisplayName),
                    PlayerId = arg.PlayerSender.Id
                };
            }
            else
            {
                packet = new ServerMessagePacket
                {
                    Message = string.Format(_chatSettings.ServerChatFormat, arg.Message),
                    PlayerId = 0
                };
            }
            await player.Connection.SendAsync(packet);
        }
    }
}
