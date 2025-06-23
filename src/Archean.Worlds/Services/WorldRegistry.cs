using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Events;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Worlds.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archean.Worlds.Services;

public class WorldRegistry : IWorldRegistry
{
    private readonly IWorld _defaultWorld;
    private readonly ILogger<WorldRegistry> _logger;

    private readonly List<IWorld> _worlds = [];

    public WorldRegistry(
        ILogger<WorldRegistry> logger,
        IGlobalEventListener eventListener,
        IOptions<ServerSettings> serverSettingsOptions)
    {
        _logger = logger;
        _defaultWorld = new TestWorld(logger, eventListener, serverSettingsOptions.Value);
        _defaultWorld.LoadAsync().GetAwaiter().GetResult(); // Todo
        _worlds.Add(_defaultWorld);
    }

    public IWorld GetDefaultWorld()
    {
        return _defaultWorld;
    }

    public async Task TransferPlayerAsync(IPlayer player, IWorld destinationWorld)
    {
        _logger.LogInformation("Transferring player {username} to world {world}",
            player.Username,
            destinationWorld.Name);

        IWorld currentWorld = _defaultWorld; // Todo: Get the current world for the connection.

        await currentWorld.LeaveAsync(player);
        await destinationWorld.JoinAsync(player);
    }
}
