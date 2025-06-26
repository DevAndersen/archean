namespace Archean.Core.Settings;

public class WorldSettings
{
    public required string DefaultWorldName { get; init; } = "main";

    public required string WorldsDirectory { get; init; } = "worlds";
}
