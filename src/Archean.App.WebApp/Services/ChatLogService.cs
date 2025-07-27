using Archean.App.WebApp.Settings;
using Archean.Core;
using Archean.Core.Models.Events;
using Archean.Core.Services;
using Archean.Core.Services.Events;
using Microsoft.Extensions.Options;

namespace Archean.App.WebApp.Services;

public class ChatLogService : IDisposable
{
    private readonly FixedSizeQueue<string> _logMessages;

    private readonly IChatService _chatService;
    private readonly IGlobalEventListener _globalEventListener;

    public IReadOnlyCollection<string> Messages => _logMessages;

    public ChatLogService(IOptions<WebAppSettings> webAppSettings, IGlobalEventListener globalEventListener, IChatService chatService)
    {
        _logMessages = new FixedSizeQueue<string>(webAppSettings.Value.ChatLogCapacity, FixedSizeQueueDirection.LastToFirst);
        _chatService = chatService;
        _globalEventListener = globalEventListener;
    }

    /// <summary>
    /// Setup event registrations.
    /// </summary>
    /// <remarks>
    /// This method should be called on application startup.
    /// </remarks>
    public void StartListening()
    {
        _globalEventListener.Subscribe<MessageEvent>(CollectChatMessages);
    }

    private void CollectChatMessages(MessageEvent messageEvent)
    {
        string message = _chatService.FormatMessageEvent(messageEvent, out _, out _);
        _logMessages.Add(message);
    }

    public void Dispose()
    {
        _globalEventListener.Unsubscribe<MessageEvent>(CollectChatMessages);
    }
}
