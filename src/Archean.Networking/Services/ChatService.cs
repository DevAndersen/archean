using Archean.Core.Services;

namespace Archean.Networking.Services;

public class ChatService : IChatService
{
    private readonly IOptions<ChatSettings> _chatSettings;

    public ChatService(IOptions<ChatSettings> chatSettings)
    {
        _chatSettings = chatSettings;
    }

    public string FormatMessageEvent(MessageEvent messageEvent)
    {
        return messageEvent.PlayerSender == null
            ? string.Format(_chatSettings.Value.ServerChatFormat, messageEvent.Message)
            : string.Format(_chatSettings.Value.ChatFormat, messageEvent.Message, messageEvent.PlayerSender.DisplayName);
    }
}
