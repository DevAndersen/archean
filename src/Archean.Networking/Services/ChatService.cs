using Archean.Core.Services;

namespace Archean.Networking.Services;

public class ChatService : IChatService
{
    private readonly IOptions<ChatSettings> _chatSettings;

    public ChatService(IOptions<ChatSettings> chatSettings)
    {
        _chatSettings = chatSettings;
    }

    public string FormatMessageEvent(MessageEvent messageEvent, out bool wasMessageTruncated, out int untruncatedLength)
    {
        string formattedMessage = messageEvent.PlayerSender == null
            ? string.Format(_chatSettings.Value.ServerChatFormat, messageEvent.Message)
            : string.Format(_chatSettings.Value.ChatFormat, messageEvent.Message, messageEvent.PlayerSender.DisplayName);

        wasMessageTruncated = formattedMessage.Length > Constants.Networking.StringLength;
        untruncatedLength = formattedMessage.Length;
        return formattedMessage[..int.Min(formattedMessage.Length, Constants.Networking.StringLength)];
    }
}
