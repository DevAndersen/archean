namespace Archean.Application.Settings;

public class AliasSettings
{
    public required bool RegisterDefaultNameAliases { get; init; } = true;

    public required bool RegisterDefaultIdAliases { get; init; } = true;

    public required Dictionary<string, string[]>? CustomAliases { get; init; }
}
