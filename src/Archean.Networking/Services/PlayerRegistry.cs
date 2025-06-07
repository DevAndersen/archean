using Archean.Core;
using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;
using Archean.Core.Settings;
using Microsoft.Extensions.Options;

namespace Archean.Networking.Services;

public class PlayerRegistry : IPlayerRegistry
{
    private readonly ServerSettings _serverSettings;

    private readonly Lock _lockObject;
    private readonly List<IPlayer> _players;

    public PlayerRegistry(IOptions<ServerSettings> serverSettingsOptions)
    {
        _serverSettings = serverSettingsOptions.Value;

        _players = [];
        _lockObject = new Lock();
    }

    public IEnumerable<IPlayer> GetAll()
    {
        lock (_lockObject)
        {
            return _players;
        }
    }

    public void Remove(IPlayer player)
    {
        lock (_lockObject)
        {
            _players.Remove(player);
        }
    }

    public void Remove(IConnection connection)
    {
        lock (_lockObject)
        {
            _players.RemoveAll(x => x.Connection == connection);
        }
    }

    public bool TryAdd(IPlayer player)
    {
        lock (_lockObject)
        {
            sbyte? availablePlayerId = TryGetAvailablePlayerId();

            if (availablePlayerId.HasValue)
            {
                _players.Add(player);
                player.Id = availablePlayerId.Value;
            }

            return availablePlayerId != null;
        }
    }

    private sbyte? TryGetAvailablePlayerId()
    {
        int highestPlayerId = Math.Min(_serverSettings.MaxPlayers - 1, Constants.Players.HighestPlayerId);

        sbyte[] playerIdsInUse = _players.Select(x => x.Id).ToArray();

        for (int i = 1; i <= highestPlayerId; i++)
        {
            sbyte sb = (sbyte)i;
            if (!playerIdsInUse.Contains(sb))
            {
                return sb;
            }
        }
        return null;
    }
}
