namespace Archean.Application.Settings;

public class ChatSettings
{
    /// <summary>
    /// The message format for player chat.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>{0}</c> will be replaced by the chat message.</item>
    /// <item><c>{1}</c> will be replaced by the player name.</item>
    /// </list>
    /// </remarks>
    public required string ChatFormat { get; init; } = "<{1}> {0}";

    /// <summary>
    /// The message format for non-player chat.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><c>{0}</c> will be replaced by the chat message.</item>
    /// </list>
    /// </remarks>
    public required string ServerChatFormat { get; init; } = "[Server] {0}";
}
