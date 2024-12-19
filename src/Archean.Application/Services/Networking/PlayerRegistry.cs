using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;
using System.Diagnostics.CodeAnalysis;

namespace Archean.Application.Services.Networking;

public class PlayerRegistry : IPlayerRegistry
{
    private readonly List<IPlayer> players;
    private readonly ILogger<PlayerRegistry> logger;

    public PlayerRegistry(ILogger<PlayerRegistry> logger)
    {
        players = [];
        this.logger = logger;
    }

    public IEnumerable<IPlayer> GetAll()
    {
        return players;
    }

    public void Remove(IPlayer player)
    {
        players.Remove(player);
    }

    public void Remove(IConnection connection)
    {
        players.RemoveAll(x => x.Connection == connection);
    }

    public bool TryAdd(IPlayer player, [NotNullWhen(false)] out string? errorMessage)
    {
        // Todo
        players.Add(player);
        errorMessage = null;

        logger.LogInformation("Added player {username} to player registry", player.Username);

        return true;
    }
}
