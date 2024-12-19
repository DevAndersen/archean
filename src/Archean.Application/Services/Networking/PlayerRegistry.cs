using Archean.Core.Models;
using Archean.Core.Models.Networking;
using Archean.Core.Services.Networking;

namespace Archean.Application.Services.Networking;

public class PlayerRegistry : IPlayerRegistry
{
    private readonly Lock lockObject;
    private readonly List<IPlayer> players;

    public PlayerRegistry()
    {
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
        sbyte[] playerIdsInUse = players.Select(x => x.Id).ToArray();

        for (int i = 0; i <= Constants.Players.HighestPlayerId; i++)
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
