namespace Archean.Core.Models.Worlds;

public interface IWorld
{
    public string Name { get; }

    public IReadOnlyList<IPlayer> Players { get; }

    public Task JoinAsync(IPlayer player);

    public Task LeaveAsync(IPlayer player);

    public Task LoadAsync();

    public Task UnloadAsync();
}
