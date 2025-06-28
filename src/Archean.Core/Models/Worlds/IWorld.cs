namespace Archean.Core.Models.Worlds;

public interface IWorld
{
    string Name { get; }

    bool IsLoaded { get; }

    IReadOnlyList<IPlayer> Players { get; }

    BlockMap? Blocks { get; }

    Task JoinAsync(IPlayer player);

    Task LeaveAsync(IPlayer player);

    Task<bool> LoadAsync();

    Task UnloadAsync();
}
