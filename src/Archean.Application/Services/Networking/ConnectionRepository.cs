using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class ConnectionRepository : IConnectionRepository
{
    private readonly List<IConnection> clients;

    public ConnectionRepository()
    {
        clients = [];
    }

    public IEnumerable<IConnection> GetConnections()
    {
        return clients;
    }

    public void Remove(IConnection connection)
    {
        clients.Remove(connection);
    }

    public bool TryAddConnection(IConnection connection)
    {
        // Todo
        clients.Add(connection);
        return true;
    }
}
