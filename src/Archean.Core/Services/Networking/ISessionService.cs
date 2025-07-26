using Archean.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Networking;

public interface ISessionService
{
    /// <summary>
    /// Returns the <see cref="IPlayer"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    IPlayer? GetPlayer();

    /// <summary>
    /// Attempts to retrieve the <see cref="IPlayer"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    bool TryGetPlayer([NotNullWhen(true)] out IPlayer? player);

    /// <summary>
    /// Sets the <see cref="IPlayer"/> of the current scope.
    /// </summary>
    /// <param name="connection"></param>
    void SetPlayer(IPlayer player);
}
