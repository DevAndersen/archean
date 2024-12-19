using Archean.Core.Models.Networking;

namespace Archean.Core.Models;

public interface IPlayer
{
    public IConnection Connection { get; }

    public string DisplayName { get; }

    public string Username { get; }
}
