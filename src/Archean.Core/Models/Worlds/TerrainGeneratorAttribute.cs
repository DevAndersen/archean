using Archean.Core.Services.Worlds;

namespace Archean.Core.Models.Worlds;

/// <summary>
/// Indicates that the decorated class is a terrain generator, as well as specifying its name.
/// </summary>
/// <remarks>
/// This is only exepcted to be used on classes inheriting from <see cref="ITerrainGenerator"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TerrainGeneratorAttribute : Attribute
{
    public string Name { get; }

    public string[]? Aliases { get; init; }

    public TerrainGeneratorAttribute(string name)
    {
        Name = name;
    }
}
