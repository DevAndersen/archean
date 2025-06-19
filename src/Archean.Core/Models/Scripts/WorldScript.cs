using Archean.Core.Models.Worlds;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Models.Scripts;

public abstract class WorldScript : Script
{
    [NotNull]
    public IWorld? World { get; set; }
}
