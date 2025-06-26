using Archean.Core.Models.Worlds;
using Archean.Core.Services;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Microsoft.Extensions.Options;

namespace Archean.Worlds.Services;

public class WorldRegistrationStartupService : IStartupService
{
    private readonly IOptions<WorldSettings> _worldSettings;
    private readonly IWorldRegistry _worldRegistry;
    private readonly WorldFactory _worldFactory;

    public WorldRegistrationStartupService(IOptions<WorldSettings> worldSettings, IWorldRegistry worldRegistry, WorldFactory worldFactory)
    {
        _worldSettings = worldSettings;
        _worldRegistry = worldRegistry;
        _worldFactory = worldFactory;
    }

    public async Task OnStartupAsync()
    {
        IWorld defaultWorld = _worldFactory.CreateNewWorld(_worldSettings.Value.DefaultWorldName); // Todo
        await _worldRegistry.AddWorldAsync(_worldSettings.Value.DefaultWorldName, defaultWorld); // Todo

        // Todo: Discover and add worlds

        // Todo: Create default world if missing

        foreach (IWorld world in _worldRegistry.GetWorlds())
        {
            await world.LoadAsync();
        }

        // Todo: Unload worlds on server shutdown
    }
}
