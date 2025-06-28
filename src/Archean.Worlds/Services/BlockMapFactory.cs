using Archean.Core.Models;
using Archean.Core.Models.Worlds;

namespace Archean.Worlds.Services;

public class BlockMapFactory
{
    public BlockMap CreateBlockMap(short width, short height, short depth)
    {
        BlockMap blocks = new BlockMap(width, height, depth);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < 7; y++)
                {
                    blocks[x, y, z] = Block.Dirt;
                }

                blocks[x, 7, z] = Block.Grass;
            }
        }

        return blocks;
    }
}
