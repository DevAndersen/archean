namespace Archean.Core.Models.Worlds;

public interface IWorld
{
    string Name { get; }

    IReadOnlyList<IPlayer> Players { get; }

    Task JoinAsync(IPlayer player);

    Task LeaveAsync(IPlayer player);

    Task LoadAsync();

    Task UnloadAsync();
}
