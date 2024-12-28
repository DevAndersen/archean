namespace Archean.Application.Settings;

public class ChatSettings
{
    public required string ChatFormat { get; init; } = "<{1}> {0}";

    public required string ServerChatFormat { get; init; } = "[Server] {0}";
}
