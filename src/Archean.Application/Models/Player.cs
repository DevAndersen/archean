using Archean.Core.Models;
using Archean.Core.Models.Networking;

namespace Archean.Application.Models;

public class Player : IPlayer
{
    public sbyte Id { get; set; }

    public IConnection Connection { get; }

    public string Username { get; }

    public string DisplayName => Username;

    public Player(IConnection connection, string username)
    {
        Connection = connection;
        Username = username;
    }
}
