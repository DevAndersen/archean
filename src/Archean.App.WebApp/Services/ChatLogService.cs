using Archean.App.WebApp.Settings;
using Archean.Core;
using Archean.Core.Models.Events;
using Archean.Core.Services;
using Archean.Core.Services.Events;
using Microsoft.Extensions.Options;

namespace Archean.App.WebApp.Services;

public class ChatLogService : IDisposable
{
    private readonly RollingQueue<string> _chatMessages;

    private readonly IChatService _chatService;
    private readonly IGlobalEventListener _globalEventListener;

    public IReadOnlyCollection<string> Messages => _chatMessages;

    public ChatLogService(IOptions<WebAppSettings> webAppSettings, IGlobalEventListener globalEventListener, IChatService chatService)
    {
        _chatMessages = new RollingQueue<string>(webAppSettings.Value.ChatLogCapacity, RollingQueueDirection.LastToFirst);
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

    /// <summary>
    /// Format incoming chat messages, and add them to the chat log.
    /// </summary>
    /// <param name="messageEvent"></param>
    private void CollectChatMessages(MessageEvent messageEvent)
    {
        string message = _chatService.FormatMessageEvent(messageEvent, out _, out _);
        _chatMessages.Add(message);
    }

    /// <summary>
    /// Clear the chat message log.
    /// </summary>
    public void Clear()
    {
        _chatMessages.Clear();
    }

    public void Dispose()
    {
        _globalEventListener.Unsubscribe<MessageEvent>(CollectChatMessages);
    }
}
