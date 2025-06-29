using Archean.Core.Models.Worlds;

namespace Archean.Core.Services.Worlds;

public interface ITerrainGenerator
{
    void PopulateBlockMap(BlockMap blocks, short width, short height, short depth);
}
