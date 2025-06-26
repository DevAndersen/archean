using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Worlds.Services;

public class WorldRegistry : IWorldRegistry
{
    private readonly ILogger<WorldRegistry> _logger;
    private readonly IOptions<WorldSettings> _worldSettings;

    private readonly Dictionary<string, IWorld> _worlds = [];

    public WorldRegistry(
        ILogger<WorldRegistry> logger,
        IOptions<WorldSettings> worldSettings)
    {
        _logger = logger;
        _worldSettings = worldSettings;
    }

    public Task<bool> AddWorldAsync(string name, IWorld world)
    {
        return Task.FromResult(_worlds.TryAdd(name, world));
    }

    public Task<bool> CreateWorldAsync(string name)
    {
        // Todo: Create world

        return Task.FromResult(true);
    }

    public async Task<bool> DeleteWorldAsync(IWorld world)
    {
        if (world == GetDefaultWorld())
        {
            return false;
        }

        await world.UnloadAsync();

        // Todo: Delete world

        return true;
    }

    public IWorld? GetDefaultWorld()
    {
        return TryGetWorld(_worldSettings.Value.DefaultWorldName, out IWorld? world) ? world : null;
    }

    public IEnumerable<IWorld> GetWorlds()
    {
        return _worlds.Values;
    }

    public Task<bool> RemoveWorldAsync(string name)
    {
        return Task.FromResult(_worlds.Remove(name));
    }

    public async Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld)
    {
        _logger.LogInformation("Transferring player {username} to world {world}",
            player.Username,
            destinationWorld.Name);

        IWorld? currentWorld = GetDefaultWorld(); // Todo: Get the current world for the connection.
        if (currentWorld != null)
        {
            await currentWorld.LeaveAsync(player);
        }

        await destinationWorld.JoinAsync(player);
    }

    public bool TryGetWorld(string name, [NotNullWhen(true)] out IWorld? world)
    {
        return _worlds.TryGetValue(name, out world);
    }
}
