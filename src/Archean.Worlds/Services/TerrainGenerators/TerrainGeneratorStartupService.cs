using Archean.Core.Services;
using Archean.Core.Services.Worlds;

namespace Archean.Worlds.Services.TerrainGenerators;

public class TerrainGeneratorStartupService : IStartupService
{
    private readonly IEnumerable<ITerrainGenerator> _terrainGenerators;
    private readonly ITerrainGeneratorRegistry _terrainGeneratorRegistry;

    public TerrainGeneratorStartupService(
        IEnumerable<ITerrainGenerator> terrainGenerators,
        ITerrainGeneratorRegistry terrainGeneratorRegistry)
    {
        _terrainGenerators = terrainGenerators;
        _terrainGeneratorRegistry = terrainGeneratorRegistry;
    }

    public Task OnStartupAsync()
    {
        foreach (ITerrainGenerator terrainGenerator in _terrainGenerators)
        {
            _terrainGeneratorRegistry.RegisterTerrainGenerator(terrainGenerator);
        }

        return Task.CompletedTask;
    }
}
