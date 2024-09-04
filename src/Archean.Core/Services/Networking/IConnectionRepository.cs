using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IConnectionRepository
{
    public IEnumerable<IConnection> GetConnections();

    public bool TryAddConnection(IConnection connection);

    public void Remove(IConnection connection);
}
