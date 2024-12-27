namespace Archean.Application.Settings;

public class ServerSettings
{
    public required ushort Port { get; init; } = 25565;

    public required int Backlog { get; init; } = 3;
}
