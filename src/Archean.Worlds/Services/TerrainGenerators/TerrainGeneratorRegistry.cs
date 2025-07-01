using Archean.Core.Models.Worlds;
using Archean.Core.Services.Worlds;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Archean.Worlds.Services.TerrainGenerators;

public class TerrainGeneratorRegistry : ITerrainGeneratorRegistry
{
    private readonly ILogger<TerrainGeneratorRegistry> _logger;
    private readonly Dictionary<string, ITerrainGenerator> _registrations = new Dictionary<string, ITerrainGenerator>(StringComparer.OrdinalIgnoreCase);

    public TerrainGeneratorRegistry(ILogger<TerrainGeneratorRegistry> logger)
    {
        _logger = logger;
    }

    public IEnumerable<ITerrainGenerator> GetTerrainGenerators()
    {
        return _registrations.Values;
    }

    public bool RegisterTerrainGenerator(ITerrainGenerator terrainGenerator)
    {
        TerrainGeneratorAttribute? attribute = terrainGenerator.GetType().GetCustomAttribute<TerrainGeneratorAttribute>();
        if (attribute == null)
        {
            _logger.LogWarning("Failed to register terrain generator of type {typeName}, type was not decorated with {attributeTypeName}",
                terrainGenerator.GetType().FullName,
                typeof(TerrainGeneratorAttribute).FullName);

            return false;
        }

        if (string.IsNullOrWhiteSpace(attribute.Name))
        {
            _logger.LogWarning("Failed to register terrain generator of type {typeName}, name was null or whitespace",
                terrainGenerator.GetType().FullName);

            return false;
        }

        if (!_registrations.TryAdd(attribute.Name, terrainGenerator))
        {
            _logger.LogWarning("Attempted to register already registered terrain generator name {terrainGeneratorName} of type {commandTypeName}",
                attribute.Name,
                typeof(TerrainGeneratorAttribute).FullName);

            return false;
        }

        return true;
    }

    public bool TryGetTerrainGenerator(string name, [NotNullWhen(true)] out ITerrainGenerator? generator)
    {
        return _registrations.TryGetValue(name, out generator);
    }
}
