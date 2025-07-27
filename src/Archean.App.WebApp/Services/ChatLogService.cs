using Archean.App.WebApp.Settings;
using Archean.Core;
using Archean.Core.Models.Events;
using Archean.Core.Services;
using Archean.Core.Services.Events;
using Microsoft.Extensions.Options;

namespace Archean.App.WebApp.Services;

public class ChatLogService
{
    private readonly FixedSizeQueue<string> _logMessages;

    private readonly IChatService _chatService;

    public IReadOnlyCollection<string> Messages => _logMessages;

    public ChatLogService(IOptions<WebAppSettings> webAppSettings, IGlobalEventListener globalEventListener, IChatService chatService)
    {
        _logMessages = new FixedSizeQueue<string>(webAppSettings.Value.ChatLogCapacity, FixedSizeQueueDirection.LastToFirst);
        globalEventListener.Subscribe<MessageEvent>(DoWork);
        _chatService = chatService;
    }

    private void DoWork(MessageEvent messageEvent)
    {
        string message = _chatService.FormatMessageEvent(messageEvent);
        _logMessages.Add(message);
    }
}
