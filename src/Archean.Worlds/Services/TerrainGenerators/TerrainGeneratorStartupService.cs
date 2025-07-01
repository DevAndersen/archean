using Archean.Core.Services;
using Archean.Core.Services.Worlds;
using Microsoft.Extensions.Logging;

namespace Archean.Worlds.Services.TerrainGenerators;

public class TerrainGeneratorStartupService : IStartupService
{
    private readonly IEnumerable<ITerrainGenerator> _terrainGenerators;
    private readonly ITerrainGeneratorRegistry _terrainGeneratorRegistry;
    private readonly ILogger<TerrainGeneratorStartupService> _logger;

    public TerrainGeneratorStartupService(
        IEnumerable<ITerrainGenerator> terrainGenerators,
        ITerrainGeneratorRegistry terrainGeneratorRegistry,
        ILogger<TerrainGeneratorStartupService> logger)
    {
        _terrainGenerators = terrainGenerators;
        _terrainGeneratorRegistry = terrainGeneratorRegistry;
        _logger = logger;
    }

    public Task OnStartupAsync()
    {
        foreach (ITerrainGenerator terrainGenerator in _terrainGenerators)
        {
            _terrainGeneratorRegistry.RegisterTerrainGenerator(terrainGenerator);
        }

        _logger.LogDebug("Registered {terrainGeneratorCount} terrain generators",
            _terrainGeneratorRegistry.GetTerrainGenerators().Count());

        return Task.CompletedTask;
    }
}
