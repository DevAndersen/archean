using Archean.Core.Models;

namespace Archean.Core.Services;

/// <summary>
/// Maps block names, string IDs, and aliases, to <see cref="Block"/> values.
/// </summary>
public interface IBlockDictionary
{
    /// <summary>
    /// Register a block identity in the dictionary.
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="block"></param>
    /// <returns><c>true</c> if <paramref name="block"/> is a defined block, otherwise <c>false</c>.</returns>
    public bool RegisterBlock(string identity, Block block);

    /// <summary>
    /// Attempt to find a <see cref="Block"/> registered by <paramref name="identity"/>.
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public bool TryGetBlock(string identity, out Block block);
}
