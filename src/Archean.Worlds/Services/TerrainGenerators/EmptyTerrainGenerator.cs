using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;

namespace Archean.Worlds.Services.TerrainGenerators;

[TerrainGenerator("Empty")]
public class EmptyTerrainGenerator : ITerrainGenerator
{
    public void PopulateBlockMap(BlockMap blocks, short width, short height, short depth)
    {
    }
}
