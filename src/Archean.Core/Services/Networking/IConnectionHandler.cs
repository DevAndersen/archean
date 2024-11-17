using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IConnectionHandler
{
    public Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken);
}
