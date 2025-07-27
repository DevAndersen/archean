using Archean.Core.Services;
using Archean.Core.Services.Commands;

namespace Archean.Networking.Services;

public class ClientEventHandler : IClientEventHandler, IDisposable
{
    private readonly IScopedEventListener _eventListener;
    private readonly ISessionService _sessionService;
    private readonly ICommandInvoker _commandInvoker;
    private readonly IChatService _chatService;

    public ClientEventHandler(
        IScopedEventListener eventListener,
        ISessionService sessionService,
        ICommandInvoker commandInvoker,
        IChatService chatService)
    {
        _eventListener = eventListener;
        _sessionService = sessionService;
        _commandInvoker = commandInvoker;
        _chatService = chatService;
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
            await player.Connection.SendAsync(new ServerMessagePacket
            {
                Message = _chatService.FormatMessageEvent(arg),
                PlayerId = arg.PlayerSender == null
                    ? (sbyte)0 // Todo: Client self ID
                    : arg.PlayerSender.Id
            });
        }
    }

    public void Dispose()
    {
        _eventListener.Unsubscribe<MessageEvent>(ReceiveMessage);
    }
}
