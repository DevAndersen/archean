using Archean.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Networking;

public interface IPlayerService
{
    /// <summary>
    /// Returns the <see cref="IPlayer"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    public IPlayer? GetPlayer();

    /// <summary>
    /// Attempts to retrieve the <see cref="IPlayer"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    public bool TryGetPlayer([NotNullWhen(true)] out IPlayer? player);

    /// <summary>
    /// Sets the <see cref="IPlayer"/> of the current scope.
    /// </summary>
    /// <param name="connection"></param>
    public void SetPlayer(IPlayer player);
}
