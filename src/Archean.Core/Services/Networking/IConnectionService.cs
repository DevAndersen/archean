using Archean.Core.Models.Networking;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Core.Services.Networking;

public interface IConnectionService
{
    /// <summary>
    /// Returns the <see cref="IConnection"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    public IConnection? GetConnection();

    /// <summary>
    /// Attempts to retrieve the <see cref="IConnection"/> of the current scope, if one has been set.
    /// </summary>
    /// <returns></returns>
    public bool TryGetConnection([NotNullWhen(true)] out IConnection? connection);

    /// <summary>
    /// Sets the <see cref="IConnection"/> of the current scope.
    /// </summary>
    /// <param name="connection"></param>
    public void SetConnection(IConnection connection);
}
