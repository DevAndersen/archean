namespace Archean.Application.Services.Networking;

public class PlayerRegistry : IPlayerRegistry
{
    private readonly ServerSettings serverSettings;

    private readonly Lock lockObject;
    private readonly List<IPlayer> players;

    public PlayerRegistry(IOptions<ServerSettings> serverSettingsOptions)
    {
        serverSettings = serverSettingsOptions.Value;

        players = [];
        lockObject = new Lock();
    }

    public IEnumerable<IPlayer> GetAll()
    {
        lock (lockObject)
        {
            return players;
        }
    }

    public void Remove(IPlayer player)
    {
        lock (lockObject)
        {
            players.Remove(player);
        }
    }

    public void Remove(IConnection connection)
    {
        lock (lockObject)
        {
            players.RemoveAll(x => x.Connection == connection);
        }
    }

    public bool TryAdd(IPlayer player)
    {
        lock (lockObject)
        {
            sbyte? availablePlayerId = TryGetAvailablePlayerId();

            if (availablePlayerId.HasValue)
            {
                players.Add(player);
                player.Id = availablePlayerId.Value;
            }

            return availablePlayerId != null;
        }
    }

    private sbyte? TryGetAvailablePlayerId()
    {
        int highestPlayerId = Math.Min(serverSettings.MaxPlayers - 1, Constants.Players.HighestPlayerId);

        sbyte[] playerIdsInUse = players.Select(x => x.Id).ToArray();

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
