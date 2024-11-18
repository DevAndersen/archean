using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Application.Services.Networking;

public class ConnectionService : IConnectionService
{
    private IConnection? connection;

    public IConnection? GetConnection()
    {
        return connection;
    }

    public bool TryGetConnection([NotNullWhen(true)] out IConnection? connection)
    {
        connection = this.connection;
        return connection != null;
    }

    public void SetConnection(IConnection connection)
    {
        this.connection = connection;
    }
}
