using Archean.Core.Models;
using Archean.Core.Models.Networking;

namespace Archean.Core.Services.Networking;

public interface IPlayerRegistry
{
    public IEnumerable<IPlayer> GetAll();

    public bool TryAdd(IPlayer player);

    public void Remove(IPlayer player);

    public void Remove(IConnection connection);
}
