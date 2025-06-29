using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;
using Microsoft.Extensions.Logging;

namespace Archean.Worlds.Services;

public class BlockMapFactory
{
    private readonly ITerrainGeneratorRegistry _terrainGeneratorRegistry;
    private readonly ILogger<BlockMapFactory> _logger;

    public BlockMapFactory(
        ITerrainGeneratorRegistry terrainGeneratorRegistry,
        ILogger<BlockMapFactory> logger)
    {
        _terrainGeneratorRegistry = terrainGeneratorRegistry;
        _logger = logger;
    }

    public BlockMap? CreateBlockMap(string terrainGenerator, short width, short height, short depth)
    {
        if (!_terrainGeneratorRegistry.TryGetTerrainGenerator(terrainGenerator, out ITerrainGenerator? generator))
        {
            _logger.LogError("Unable to find terrain generator '{terrainGenerator}'",
                terrainGenerator);

            return null;
        }

        BlockMap blocks = new BlockMap(width, height, depth);
        generator.PopulateBlockMap(blocks, width, height, depth);

        return blocks;
    }
}
