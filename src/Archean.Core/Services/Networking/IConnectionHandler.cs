using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IConnectionHandler
{
    Task HandleNewConnectionAsync(IConnection connection, CancellationToken cancellationToken);
}
