using Archean.Core.Models.Worlds;
using Archean.Core.Services;
using Archean.Core.Services.Worlds;
using Archean.Core.Settings;
using Archean.Worlds.Services.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archean.Worlds.Services;

public class WorldRegistrationStartupService : IStartupService
{
    private readonly IOptions<WorldSettings> _worldSettings;
    private readonly IWorldRegistry _worldRegistry;
    private readonly WorldFactory _worldFactory;
    private readonly ILogger<WorldRegistrationStartupService> _logger;
    private readonly BlockMapPersistenceHandler _blockMapPersistenceHandler;
    private readonly BlockMapFactory _blockMapFactory;

    public WorldRegistrationStartupService(
        IOptions<WorldSettings> worldSettings,
        IWorldRegistry worldRegistry,
        WorldFactory worldFactory,
        ILogger<WorldRegistrationStartupService> logger,
        BlockMapPersistenceHandler blockMapPersistenceHandler,
        BlockMapFactory blockMapFactory)
    {
        _worldSettings = worldSettings;
        _worldRegistry = worldRegistry;
        _worldFactory = worldFactory;
        _logger = logger;
        _blockMapPersistenceHandler = blockMapPersistenceHandler;
        _blockMapFactory = blockMapFactory;
    }

    public async Task OnStartupAsync()
    {
        Directory.CreateDirectory(_worldSettings.Value.WorldsDirectory);

        foreach (string worldDirectory in Directory.GetDirectories(_worldSettings.Value.WorldsDirectory))
        {
            string worldName = Path.GetFileName(worldDirectory);
            IWorld world = _worldFactory.CreateNewWorld(worldName);
            await _worldRegistry.RegisterWorldAsync(world);
        }

        IWorld? defaultWorld = _worldRegistry.GetWorlds().FirstOrDefault(x => x.Name == _worldSettings.Value.DefaultWorldName);
        if (defaultWorld == null)
        {
            _logger.LogInformation("Creating new default world '{defaultWorldName}'",
                _worldSettings.Value.DefaultWorldName);

            defaultWorld = _worldFactory.CreateNewWorld(_worldSettings.Value.DefaultWorldName);

            // Create default world directory
            string defaultWorldDirectory = Path.Combine(_worldSettings.Value.WorldsDirectory, _worldSettings.Value.DefaultWorldName);
            Directory.CreateDirectory(defaultWorldDirectory);

            // Create block map
            BlockMap blockMap = _blockMapFactory.CreateBlockMap(16, 16, 16);
            await _blockMapPersistenceHandler.SaveBlockMapAsync(
                Path.Combine(defaultWorldDirectory, _worldSettings.Value.BlockMapFileName),
                blockMap);

            await _worldRegistry.RegisterWorldAsync(defaultWorld);
        }

        if (!await defaultWorld.LoadAsync())
        {
            Exception e = new Exception("Failed to load default world");
            _logger.LogError(e, "Failed to load default world");
            throw e;
        }

        // Todo: Unload worlds on server shutdown
    }
}
