namespace Archean.Application.Settings;

public class ServerSettings
{
    public required ushort Port { get; init; } = 25565;

    public required int Backlog { get; init; } = 3;

    public required string Name { get; init; }

    public required string Motd { get; init; }

    public required string WorldLoadingMotd { get; init; }
}
