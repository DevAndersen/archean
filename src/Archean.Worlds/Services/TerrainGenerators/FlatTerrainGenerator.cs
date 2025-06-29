using Archean.Core.Models;
using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;

namespace Archean.Worlds.Services.TerrainGenerators;

[TerrainGenerator("Flat")]
public class FlatTerrainGenerator : ITerrainGenerator
{
    public void PopulateBlockMap(BlockMap blocks, short width, short height, short depth)
    {
        int grassLayer = (height / 2) - 1;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < grassLayer; y++)
                {
                    blocks[x, y, z] = Block.Dirt;
                }

                blocks[x, grassLayer, z] = Block.Grass;
            }
        }
    }
}
