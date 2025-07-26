using Archean.Core.Services.Commands;

namespace Archean.Networking.Services;

public class ClientEventHandler : IClientEventHandler, IDisposable
{
    private readonly IScopedEventListener _eventListener;
    private readonly ISessionService _sessionService;
    private readonly ChatSettings _chatSettings;
    private readonly ICommandInvoker _commandInvoker;

    public ClientEventHandler(
        IScopedEventListener eventListener,
        ISessionService sessionService,
        IOptions<ChatSettings> chatSettingsOptions,
        ICommandInvoker commandInvoker)
    {
        _eventListener = eventListener;
        _sessionService = sessionService;
        _chatSettings = chatSettingsOptions.Value;
        _commandInvoker = commandInvoker;
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

            if (!await _commandInvoker.TryInvokeCommandAsync(commandText, arg.PlayerSender))
            {
                if (_sessionService.TryGetPlayer(out IPlayer? player))
                {
                    await player.Connection.SendAsync(new ServerMessagePacket
                    {
                        Message = "No such command was found",
                        PlayerId = 0 // Todo: Client self ID
                    });
                }
            }
        }
        else if (_sessionService.TryGetPlayer(out IPlayer? player))
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
                    PlayerId = 0 // Todo: Client self ID
                };
            }
            await player.Connection.SendAsync(packet);
        }
    }

    public void Dispose()
    {
        _eventListener.Unsubscribe<MessageEvent>(ReceiveMessage);
    }
}
