using Archean.Core.Models.Networking;

namespace Archean.Core.Models;

public interface IPlayer
{
    public sbyte Id { get; set; }

    public IConnection Connection { get; }

    public string DisplayName { get; }

    public string Username { get; }
}
