using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Worlds;

public interface ITerrainGeneratorRegistry
{
    bool RegisterTerrainGenerator(ITerrainGenerator terrainGenerator);

    bool TryGetTerrainGenerator(string name, [NotNullWhen(true)] out ITerrainGenerator? generator);
}
