namespace Archean.Networking.Services;

public class PlayerService : IPlayerService
{
    private readonly ServerSettings _serverSettings;

    private readonly Lock _lockObject;
    private readonly Dictionary<string, IPlayer> _players;

    public PlayerService(IOptions<ServerSettings> serverSettingsOptions)
    {
        _serverSettings = serverSettingsOptions.Value;

        _players = [];
        _lockObject = new Lock();
    }

    public IEnumerable<IPlayer> GetAll()
    {
        lock (_lockObject)
        {
            return _players.Values;
        }
    }

    public void Remove(IPlayer player)
    {
        Remove(player.Connection);
    }

    public void Remove(IConnection connection)
    {
        lock (_lockObject)
        {
            IEnumerable<KeyValuePair<string, IPlayer>> matchingPlayers = _players.Where(x => x.Value.Connection == connection);
            foreach (KeyValuePair<string, IPlayer> player in matchingPlayers)
            {
                _players.Remove(player.Key);
            }
        }
    }

    public bool TryAdd(IPlayer player)
    {
        lock (_lockObject)
        {
            sbyte? availablePlayerId = TryGetAvailablePlayerId();

            if (availablePlayerId.HasValue)
            {
                if (_players.TryAdd(player.Username, player))
                {
                    player.Id = availablePlayerId.Value;
                }
                else
                {
                    return false;
                }
            }

            return availablePlayerId != null;
        }
    }

    private sbyte? TryGetAvailablePlayerId()
    {
        int highestPlayerId = Math.Min(_serverSettings.MaxPlayers - 1, Constants.Players.HighestPlayerId);

        sbyte[] playerIdsInUse = _players.Select(x => x.Value.Id).ToArray();

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
