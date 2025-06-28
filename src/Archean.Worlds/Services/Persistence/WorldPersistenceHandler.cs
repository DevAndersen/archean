using Archean.Core.Models.Worlds;
using Archean.Core.Settings;
using Archean.Worlds.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Archean.Worlds.Services.Persistence;

public class WorldPersistenceHandler
{
    private readonly IOptions<WorldSettings> _worldSettings;
    private readonly ILogger<WorldPersistenceHandler> _logger;
    private readonly BlockMapPersistenceHandler _blockMapPersistenceHandler;

    public WorldPersistenceHandler(
        IOptions<WorldSettings> worldSettings,
        ILogger<WorldPersistenceHandler> logger,
        BlockMapPersistenceHandler blockMapPersistenceHandler)
    {
        _worldSettings = worldSettings;
        _logger = logger;
        _blockMapPersistenceHandler = blockMapPersistenceHandler;
    }

    public async Task<bool> SaveWorldAsync(TestWorld world)
    {
        try
        {
            if (world.Blocks != null)
            {
                await _blockMapPersistenceHandler.SaveBlockMapAsync(GetBlockMapFilePath(world), world.Blocks);
            }
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save world {worldName}",
                world.Name);

            return false;
        }
    }

    public async Task<bool> LoadWorldAsync(TestWorld world)
    {
        try
        {
            BlockMap? blockMap = await _blockMapPersistenceHandler.LoadBlockMapAsync(GetBlockMapFilePath(world));
            if (blockMap == null)
            {
                return false;
            }

            world.Blocks = blockMap;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load world {worldName}",
                world.Name);

            return false;
        }
    }

    private string GetBlockMapFilePath(TestWorld world)
    {
        return Path.Join(
            _worldSettings.Value.WorldsDirectory,
            world.Name,
            _worldSettings.Value.BlockMapFileName);
    }
}
