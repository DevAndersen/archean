namespace Archean.Application.Settings;

public class ServerSettings
{
    /// <summary>
    /// The port that the socket server runs on.
    /// </summary>
    public required ushort Port { get; init; } = 25565;

    /// <summary>
    /// The size of the pending connection backlog.
    /// </summary>
    public required int Backlog { get; init; } = 3;

    /// <summary>
    /// The name of the server.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The MOTD (message of the day) of the server.
    /// </summary>
    public required string Motd { get; init; }

    /// <summary>
    /// The MOTD displayed when a player joins the world.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Use "<c>{0}</c>" to show the world loading percentage.</item>
    /// </list>
    /// </remarks>
    public required string WorldLoadingMotd { get; init; }

    /// <summary>
    /// The disconnect message sent to the client when, when attempting to join the server while it is full.
    /// </summary>
    public required string ServerFullMessage { get; init; }
}
